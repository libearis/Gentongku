using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Abstractions;
using Scheduler.Infrastructure.Jobs;
using Scheduler.Infrastructure.Persistence;

namespace Scheduler.Infrastructure;

public static class SchedulerModule
{
    public static IServiceCollection AddSchedulerModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<SchedulerDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", SchedulerDbContext.Schema))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<ISchedulerService, SchedulerService>();
        services.AddScoped<GenerateDummyDataJob>();

        return services;
    }
}
