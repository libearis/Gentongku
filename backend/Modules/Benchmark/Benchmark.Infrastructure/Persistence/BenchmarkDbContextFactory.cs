using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Benchmark.Infrastructure.Persistence;

/// <summary>
/// Design-time-only factory so `dotnet ef migrations add` can build the model
/// without booting the full Host (which eagerly opens real Postgres/Redis
/// connections at startup). Never used at runtime — the Host wires up
/// <see cref="BenchmarkDbContext"/> itself via BenchmarkModule.
/// </summary>
public sealed class BenchmarkDbContextFactory : IDesignTimeDbContextFactory<BenchmarkDbContext>
{
    public BenchmarkDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=gentongku;Username=postgres;Password=changeme";

        var options = new DbContextOptionsBuilder<BenchmarkDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", BenchmarkDbContext.Schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new BenchmarkDbContext(options);
    }
}
