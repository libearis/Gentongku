using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalog.Infrastructure.Persistence;

// Design-time-only, so `dotnet ef migrations add` can build the model without booting the full Host and its real Postgres/Redis connections.
public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=gentongku;Username=postgres;Password=changeme";

        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", CatalogDbContext.Schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new CatalogDbContext(options);
    }
}
