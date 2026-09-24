using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Abstractions;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Services;

namespace Ordering.Infrastructure;

public static class OrderingModule
{
    public static IServiceCollection AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<OrderingDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", OrderingDbContext.Schema))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IOrderQueries, OrderQueries>();
        services.AddScoped<IOrderingDummyDataGenerator, OrderingDummyDataGenerator>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IOrderManagementService, OrderManagementService>();

        return services;
    }
}
