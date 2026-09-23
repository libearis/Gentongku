using Benchmark.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;
using Scheduler.Infrastructure.Persistence;
using Ticketing.Infrastructure.Persistence;

namespace Gentongku.Api.Seeding;

// Entry point for `dotnet run -- migrate`; not run automatically at normal app startup.
public static class SeederData
{
    public static async Task RunAsync(string connectionString, IServiceProvider services)
    {
        EnsureDatabaseExists(connectionString);

        using (var schemaConnection = new Npgsql.NpgsqlConnection(connectionString))
        {
            schemaConnection.Open();
            using var cmd = schemaConnection.CreateCommand();
            cmd.CommandText = "CREATE SCHEMA IF NOT EXISTS logs;";
            cmd.ExecuteNonQuery();
        }

        await IdentitySeeder.SeedAsync(services.GetRequiredService<IdentityDbContext>());
        await CatalogSeeder.SeedAsync(services.GetRequiredService<CatalogDbContext>());

        await services.GetRequiredService<OrderingDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<BenchmarkDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<SchedulerDbContext>().Database.MigrateAsync();
        await services.GetRequiredService<TicketingDbContext>().Database.MigrateAsync();
    }

    private static void EnsureDatabaseExists(string targetConnectionString)
    {
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(targetConnectionString);
        var targetDatabase = builder.Database;
        if (string.IsNullOrEmpty(targetDatabase)) return;

        builder.Database = "postgres";
        using var adminConnection = new Npgsql.NpgsqlConnection(builder.ConnectionString);
        adminConnection.Open();

        using (var existsCmd = adminConnection.CreateCommand())
        {
            existsCmd.CommandText = "SELECT 1 FROM pg_database WHERE datname = @name";
            existsCmd.Parameters.AddWithValue("name", targetDatabase);
            if (existsCmd.ExecuteScalar() is not null) return;
        }

        using var createCmd = adminConnection.CreateCommand();
        createCmd.CommandText = $"CREATE DATABASE \"{targetDatabase.Replace("\"", "\"\"")}\"";
        createCmd.ExecuteNonQuery();
    }
}
