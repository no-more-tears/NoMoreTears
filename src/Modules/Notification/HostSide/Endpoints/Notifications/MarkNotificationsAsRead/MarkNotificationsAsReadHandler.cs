using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationsAsRead;

namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.MarkNotificationsAsRead;

public static class MarkNotificationsAsReadHandler
{
    public static async Task<IResult> Handle(
        [FromBody] MarkNotificationsAsReadRequest request,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new MarkNotificationsAsReadCommand(currentUserSession.GetRequiredUserId(), request.NotificationIds);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
