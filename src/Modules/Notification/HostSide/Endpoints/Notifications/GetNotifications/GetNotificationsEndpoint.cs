namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.GetNotifications;

public sealed class GetNotificationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.Base, GetNotificationsHandler.Handle)
            .WithName(EndpointName.GetNotifications)
            .WithTags(EndpointTag.Notifications)
            .RequireAuthorization();
    }
}
