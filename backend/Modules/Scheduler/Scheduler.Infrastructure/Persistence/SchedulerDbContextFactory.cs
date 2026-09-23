using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Scheduler.Infrastructure.Persistence;

// Lets `dotnet ef migrations add` build the model without booting the full Host and its real Postgres/Redis connections.
public sealed class SchedulerDbContextFactory : IDesignTimeDbContextFactory<SchedulerDbContext>
{
    public SchedulerDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=gentongku;Username=postgres;Password=changeme";

        var options = new DbContextOptionsBuilder<SchedulerDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", SchedulerDbContext.Schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new SchedulerDbContext(options);
    }
}
