using System.Text;
using Benchmark.Infrastructure.Persistence;
using BuildingBlocks.Logging;
using Catalog.Infrastructure.Persistence;
using Gentongku.Api.ModuleRegistration;
using Hangfire;
using Hangfire.PostgreSql;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NpgsqlTypes;
using Ordering.Infrastructure.Persistence;
using Scheduler.Infrastructure.Persistence;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using StackExchange.Redis;
using Ticketing.Infrastructure.Persistence;

// The Serilog Postgres sink writes DateTimeOffset.Now (local offset) into a
// `timestamp with time zone` column; Npgsql 6+ otherwise rejects any offset
// other than UTC. This machine's local offset isn't UTC, so opt into Npgsql's
// legacy (offset-converting) behavior for that column.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Load the repo-root `.env` (gitignored — see `.env.example`) into process
// env vars for local `dotnet run`. ASP.NET Core's built-in env-var config
// provider then picks up e.g. `ConnectionStrings__Default` as
// `ConnectionStrings:Default` automatically — no extra config code needed.
// docker-compose reads the same `.env` natively for its own `${VAR}`
// substitution in docker-compose.yml. A missing `.env` is fine (e.g. inside
// the container, where docker-compose passes real env vars directly).
// Search upward from the executable's directory rather than assuming a fixed
// relative depth, since that depth differs between `dotnet run` (working
// directory = project folder) and running the built binary directly.
var envSearchDir = new DirectoryInfo(AppContext.BaseDirectory);
while (envSearchDir is not null && !File.Exists(Path.Combine(envSearchDir.FullName, ".env")))
{
    envSearchDir = envSearchDir.Parent;
}
if (envSearchDir is not null)
{
    DotNetEnv.Env.Load(Path.Combine(envSearchDir.FullName, ".env"));
}

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=localhost;Port=5432;Database=gentongku;Username=postgres;Password=CHANGE_ME";

var seqUrl = builder.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341";

// The Postgres sink below auto-creates its TABLE but not its SCHEMA, so ensure
// `logs` exists first (AGENTS.md section 9: logs.application_log schema).
using (var schemaConnection = new Npgsql.NpgsqlConnection(connectionString))
{
    schemaConnection.Open();
    using var cmd = schemaConnection.CreateCommand();
    cmd.CommandText = "CREATE SCHEMA IF NOT EXISTS logs;";
    cmd.ExecuteNonQuery();
}

// ---- Serilog: Postgres sink (logs.application_log, business/job logs) + Seq (technical/infra logs) ----
// AGENTS.md section 9.
var columnOptions = new Dictionary<string, ColumnWriterBase>
{
    { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
    { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
    { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
    { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
    { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
    { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
};

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With<ApplicationEnricher>()
    .WriteTo.Console()
    .WriteTo.PostgreSQL(
        connectionString,
        tableName: "application_log",
        columnOptions,
        schemaName: "logs",
        needAutoCreateTable: true)
    .WriteTo.Seq(seqUrl)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    // ---- MVC / Controllers ----
    builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();

    // ---- CORS: allow the Angular dev server to call this API cross-origin. ----
    const string FrontendCorsPolicy = "FrontendDev";
    var frontendOrigins = builder.Configuration.GetSection("Cors:FrontendOrigins").Get<string[]>()
        ?? new[] { "http://localhost:4200" };
    builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(frontendOrigins).AllowAnyHeader().AllowAnyMethod()));

    // ---- Modules (AGENTS.md section 3: one AddXModule() per module) ----
    builder.Services.AddGentongkuModules(builder.Configuration);

    // ---- Redis ----
    var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(redisConnectionString));

    // ---- JWT auth ----
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "gentongku-dev-only-secret-please-change-0123456789";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "gentongku";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "gentongku-client";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });
    builder.Services.AddAuthorization();

    // ---- Hangfire (Postgres storage), dashboard at /hangfire (AGENTS.md section 7 & 10) ----
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));
    builder.Services.AddHangfireServer();

    // ---- Health checks: DB + Redis + Hangfire storage (AGENTS.md section 6.4) ----
    builder.Services.AddHealthChecks()
        .AddNpgSql(connectionString, name: "postgres")
        .AddRedis(redisConnectionString, name: "redis")
        .AddHangfire(_ => { }, name: "hangfire");

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseCors(FrontendCorsPolicy);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapHealthChecks("/health");

    app.UseHangfireDashboard(builder.Configuration["Hangfire:DashboardPath"] ?? "/hangfire");

    // ---- Apply migrations + seed data for every module's DbContext on startup ----
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var identityDb = services.GetRequiredService<IdentityDbContext>();
        await IdentitySeeder.SeedAsync(identityDb);

        await CatalogSeeder.SeedAsync(services.GetRequiredService<CatalogDbContext>());
        await services.GetRequiredService<OrderingDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<BenchmarkDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<SchedulerDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<TicketingDbContext>().Database.MigrateAsync();
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Gentongku.Api terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>Exposed for WebApplicationFactory-based integration tests.</summary>
public partial class Program { }
