using NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetNotifications;

namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.GetNotifications;

public static class GetNotificationsHandler
{
    public static async Task<IResult> Handle(
        [FromQuery] bool? unreadOnly,
        [FromQuery] string? category,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetNotificationsQuery(currentUserSession.GetRequiredUserId(), unreadOnly ?? false, category, pageNumber ?? 1, pageSize ?? 20);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
