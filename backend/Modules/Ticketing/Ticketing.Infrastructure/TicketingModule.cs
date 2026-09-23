using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Application.Abstractions;
using Ticketing.Application.Services;
using Ticketing.Infrastructure.Grpc;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure;

public static class TicketingModule
{
    public static IServiceCollection AddTicketingModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<TicketingDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", TicketingDbContext.Schema))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<ITicketQueries, TicketQueries>();
        services.AddScoped<IIssueIntakeClient, GrpcIssueIntakeClient>();
        services.AddScoped<ICreateTicketService, CreateTicketService>();

        return services;
    }
}
