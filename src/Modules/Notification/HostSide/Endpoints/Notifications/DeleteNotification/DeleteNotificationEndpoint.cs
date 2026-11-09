namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.DeleteNotification;

public sealed class DeleteNotificationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.Notification, DeleteNotificationHandler.Handle)
            .WithName(EndpointName.DeleteNotification)
            .WithTags(EndpointTag.Notifications)
            .RequireAuthorization();
    }
}
