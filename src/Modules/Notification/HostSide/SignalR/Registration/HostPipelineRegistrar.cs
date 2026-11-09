namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Registration;

/// <summary>
///     Configures the HTTP pipeline of the SignalR host.
/// </summary>
public static class HostPipelineRegistrar
{
    extension(WebApplication application)
    {
        public WebApplication UseSignalRHostPipeline()
        {
            application
                .UseCors(HostRegistrar.CorsPolicyName)
                .UseAuthentication()
                .UseAuthorization();

            application.MapHealthChecks("/health");
            application.MapHub<Hubs.NotificationsHub>(Contracts.Constants.NotificationHub.Route);

            return application;
        }
    }
}