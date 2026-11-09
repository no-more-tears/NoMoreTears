using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Files;
using NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Trust;
using NoMoreTears.Modules.Masters.Providers;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Authorization.Registration;
using NoMoreTears.Shared.RabbitMq.Registration;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Registration;

/// <summary>
///     Registers the Masters consumers host. Instances compete for the same queues,
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
            .AddMastersDataAccess(blueprint => blueprint.UseConnectionString(ConnectionStringName))
            .AddMastersApplications(configuration)
            .AddMastersMessaging(consumers => consumers
                .AddConsumer<MasterFileStagedEvent, MasterFileStagedEventConsumer>()
                .AddConsumer<MasterFilesObsoletedEvent, MasterFilesObsoletedEventConsumer>()
                .AddConsumer<SanctionImposedEvent, SanctionImposedEventConsumer>()
                .AddConsumer<SanctionLiftedEvent, SanctionLiftedEventConsumer>()
                .AddConsumer<TemporaryMeasureAppliedEvent, TemporaryMeasureAppliedEventConsumer>()
                .AddConsumer<TemporaryMeasureLiftedEvent, TemporaryMeasureLiftedEventConsumer>())
            .AddRabbitMq(configuration);

        return services;
    }
}
