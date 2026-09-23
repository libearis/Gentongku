using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ordering.Infrastructure.Persistence;

// Lets `dotnet ef migrations add` build the model without booting the full Host, which eagerly opens real Postgres/Redis connections.
public sealed class OrderingDbContextFactory : IDesignTimeDbContextFactory<OrderingDbContext>
{
    public OrderingDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=gentongku;Username=postgres;Password=changeme";

        var options = new DbContextOptionsBuilder<OrderingDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", OrderingDbContext.Schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new OrderingDbContext(options);
    }
}
