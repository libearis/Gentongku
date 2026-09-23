using Benchmark.Application.Abstractions;
using Benchmark.Infrastructure.Persistence;
using Benchmark.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmark.Infrastructure;

public static class BenchmarkModule
{
    public static IServiceCollection AddBenchmarkModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<BenchmarkDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", BenchmarkDbContext.Schema))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IBenchmarkService, BenchmarkService>();

        return services;
    }
}
