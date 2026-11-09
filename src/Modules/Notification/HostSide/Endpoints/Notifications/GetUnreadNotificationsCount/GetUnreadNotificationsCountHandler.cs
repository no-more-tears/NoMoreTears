using NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUnreadNotificationsCount;

namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.GetUnreadNotificationsCount;

public static class GetUnreadNotificationsCountHandler
{
    public static async Task<IResult> Handle(
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetUnreadNotificationsCountQuery(currentUserSession.GetRequiredUserId());

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
