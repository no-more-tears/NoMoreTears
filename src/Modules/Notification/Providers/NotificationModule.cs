using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Providers.Hosting;

namespace NoMoreTears.Modules.Notification.Providers;

/// <summary>
///     Notification module of the WebApi host.
/// </summary>
public sealed class NotificationModule : IModule
{
    private const string ConnectionStringName = "Postgres";

    public string Name => nameof(NotificationModule);

    public void Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddNotificationDataAccess(blueprint => blueprint.UseConnectionString(ConnectionStringName))
            .AddNotificationApplications(configuration)
            .AddNotificationEndpoints()
            .AddNotificationMessaging();
    }
}