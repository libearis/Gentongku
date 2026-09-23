using System.Text;
using BuildingBlocks.Logging;
using Gentongku.Api.ModuleRegistration;
using Gentongku.Api.Seeding;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NpgsqlTypes;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using StackExchange.Redis;

// Npgsql 6+ rejects non-UTC DateTimeOffset; the Postgres sink below writes local time, so opt into legacy behavior.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var envSearchDir = new DirectoryInfo(AppContext.BaseDirectory);
while (envSearchDir is not null && !File.Exists(Path.Combine(envSearchDir.FullName, ".env")))
{
    envSearchDir = envSearchDir.Parent;
}
if (envSearchDir is not null)
{
    DotNetEnv.Env.Load(Path.Combine(envSearchDir.FullName, ".env"));
}

var isMigrateCommand = args.Any(a => string.Equals(a, "migrate", StringComparison.OrdinalIgnoreCase));

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=localhost;Port=5432;Database=gentongku;Username=postgres;Password=CHANGE_ME";
var seqUrl = builder.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341";

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
    builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();

    const string FrontendCorsPolicy = "FrontendDev";
    var frontendOrigins = builder.Configuration.GetSection("Cors:FrontendOrigins").Get<string[]>()
        ?? new[] { "http://localhost:4200" };
    builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(frontendOrigins).AllowAnyHeader().AllowAnyMethod()));

    builder.Services.AddGentongkuModules(builder.Configuration);

    var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(redisConnectionString));

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

    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));
    builder.Services.AddHangfireServer();

    builder.Services.AddHealthChecks()
        .AddNpgSql(connectionString, name: "postgres")
        .AddRedis(redisConnectionString, name: "redis")
        .AddHangfire(_ => { }, name: "hangfire");

    var app = builder.Build();

    if (isMigrateCommand)
    {
        using var scope = app.Services.CreateScope();
        await SeederData.RunAsync(connectionString, scope.ServiceProvider);
        Log.Information("Migration + seed complete.");
        return;
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors(FrontendCorsPolicy);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health");
    app.UseHangfireDashboard(builder.Configuration["Hangfire:DashboardPath"] ?? "/hangfire");

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

public partial class Program { }
