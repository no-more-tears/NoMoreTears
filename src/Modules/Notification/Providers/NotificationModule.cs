using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Providers.Hosting;

namespace NoMoreTears.Modules.Notification.Providers;

public sealed class NotificationModule : IModule
{
    public string Name => nameof(NotificationModule);

    public void Register(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEmailNotifications(configuration.GetSection(NotificationRegistrar.SectionName));
    }
}
