namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.MarkNotificationsAsRead;

public sealed class MarkNotificationsAsReadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ReadMany, MarkNotificationsAsReadHandler.Handle)
            .WithName(EndpointName.MarkNotificationsAsRead)
            .WithTags(EndpointTag.Notifications)
            .RequireAuthorization();
    }
}
