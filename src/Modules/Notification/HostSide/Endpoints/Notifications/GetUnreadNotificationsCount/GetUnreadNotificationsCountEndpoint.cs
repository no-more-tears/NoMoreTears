namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.GetUnreadNotificationsCount;

public sealed class GetUnreadNotificationsCountEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.UnreadCount, GetUnreadNotificationsCountHandler.Handle)
            .WithName(EndpointName.GetUnreadNotificationsCount)
            .WithTags(EndpointTag.Notifications)
            .RequireAuthorization();
    }
}
