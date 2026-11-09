namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ReadAll, MarkAllNotificationsAsReadHandler.Handle)
            .WithName(EndpointName.MarkAllNotificationsAsRead)
            .WithTags(EndpointTag.Notifications)
            .RequireAuthorization();
    }
}
