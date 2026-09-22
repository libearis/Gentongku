using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Persistence;
using Identity.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", CatalogDbContext.Schema)));

        services.AddScoped<ICatalogQueries, CatalogQueries>();
        services.AddScoped<ISellerProfileService, SellerProfileService>();
        services.AddScoped<ISellerProfileProvisioner, SellerProfileProvisionerAdapter>();

        return services;
    }
}
