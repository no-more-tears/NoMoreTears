using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkAllNotificationsAsRead;

namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.MarkAllNotificationsAsRead;

public static class MarkAllNotificationsAsReadHandler
{
    public static async Task<IResult> Handle(
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new MarkAllNotificationsAsReadCommand(currentUserSession.GetRequiredUserId());

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
