using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationAsRead;

namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.MarkNotificationAsRead;

public static class MarkNotificationAsReadHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid notificationId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new MarkNotificationAsReadCommand(currentUserSession.GetRequiredUserId(), notificationId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
