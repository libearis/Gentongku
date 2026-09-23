using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Benchmark.Infrastructure.Persistence;

// Design-time-only, so `dotnet ef migrations add` can build the model without booting the full Host and its real Postgres/Redis connections.
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
