using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.DeleteNotification;

namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications.DeleteNotification;

public static class DeleteNotificationHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid notificationId,
        [FromServices] ICurrentUserSession currentUserSession,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteNotificationCommand(currentUserSession.GetRequiredUserId(), notificationId);

        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}
