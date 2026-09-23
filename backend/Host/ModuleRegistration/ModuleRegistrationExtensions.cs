using Benchmark.Infrastructure;
using Catalog.Infrastructure;
using Identity.Infrastructure;
using Ordering.Infrastructure;
using Scheduler.Infrastructure;
using Ticketing.Infrastructure;

namespace Gentongku.Api.ModuleRegistration;

// Modules never reference each other directly, only through interfaces registered here.
public static class ModuleRegistrationExtensions
{
    public static IServiceCollection AddGentongkuModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityModule(configuration);
        services.AddCatalogModule(configuration);
        services.AddOrderingModule(configuration);
        services.AddBenchmarkModule(configuration);
        services.AddSchedulerModule(configuration);
        services.AddTicketingModule(configuration);

        return services;
    }
}
