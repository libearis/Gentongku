using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ticketing.Infrastructure.Persistence;

// Design-time-only, so `dotnet ef migrations add` can build the model without booting the full Host and its real Postgres/Redis connections.
public sealed class TicketingDbContextFactory : IDesignTimeDbContextFactory<TicketingDbContext>
{
    public TicketingDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=gentongku;Username=postgres;Password=changeme";

        var options = new DbContextOptionsBuilder<TicketingDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", TicketingDbContext.Schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new TicketingDbContext(options);
    }
}
