using Benchmark.Infrastructure;
using Catalog.Infrastructure;
using Identity.Infrastructure;
using Ordering.Infrastructure;
using Scheduler.Infrastructure;
using Ticketing.Infrastructure;

namespace Gentongku.Api.ModuleRegistration;

/// <summary>
/// Composition root: wires every module's own AddXModule() extension
/// (AGENTS.md section 3 — "Host/ModuleRegistration/ one *.AddXModule()
/// extension per module"). Each module registers its own DbContext and
/// Application-layer service implementations; modules never reference each
/// other directly, only through interfaces registered here.
/// </summary>
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
