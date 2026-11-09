namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.ReadOne, MarkNotificationAsReadHandler.Handle)
            .WithName(EndpointName.MarkNotificationAsRead)
            .WithTags(EndpointTag.Notifications)
            .RequireAuthorization();
    }
}
