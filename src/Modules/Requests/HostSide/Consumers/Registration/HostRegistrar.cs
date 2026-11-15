using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Files;
using NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Masters;
using NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;
using NoMoreTears.Modules.Requests.Providers;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Authorization.Registration;
using NoMoreTears.Shared.RabbitMq.Registration;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Registration;

/// <summary>
///     Registers the Requests consumers host. Instances compete for the same queues,
///     so the host scales out by running more instances.
/// </summary>
public static class HostRegistrar
{
    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddConsumerHost(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            // Consumers act on behalf of the system, not of a user.
            .AddUserSessions(_ => new CurrentUserSession())
            .AddRequestsDataAccess(blueprint => blueprint.UseConnectionString(ConnectionStringName))
            .AddRequestsApplications(configuration)
            .AddRequestsMessaging(consumers => consumers
                // Own photos: the check and the removal of obsolete files.
                .AddConsumer<RequestMediaStagedEvent, RequestMediaStagedEventConsumer>()
                .AddConsumer<RequestFilesObsoletedEvent, RequestFilesObsoletedEventConsumer>()
                // Masters: snapshots of the catalog and of the access of masters.
                .AddConsumer<CategoryCreatedEvent, CategoryCreatedEventConsumer>()
                .AddConsumer<CategoryUpdatedEvent, CategoryUpdatedEventConsumer>()
                .AddConsumer<CategoryArchivedEvent, CategoryArchivedEventConsumer>()
                .AddConsumer<CategoryActivatedEvent, CategoryActivatedEventConsumer>()
                .AddConsumer<MasterAccessUpdatedEvent, MasterAccessUpdatedEventConsumer>()
                // Trust: disputes and publication restrictions of clients.
                .AddConsumer<IncidentOpenedEvent, IncidentOpenedEventConsumer>()
                .AddConsumer<IncidentClosedEvent, IncidentClosedEventConsumer>()
                .AddConsumer<SanctionImposedEvent, SanctionImposedEventConsumer>()
                .AddConsumer<SanctionLiftedEvent, SanctionLiftedEventConsumer>()
                .AddConsumer<TemporaryMeasureAppliedEvent, TemporaryMeasureAppliedEventConsumer>()
                .AddConsumer<TemporaryMeasureLiftedEvent, TemporaryMeasureLiftedEventConsumer>())
            .AddRabbitMq(configuration);

        return services;
    }
}