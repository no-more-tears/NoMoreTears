namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotificationsChange;

public sealed class PushNotificationsChangeCommandHandler(
    INotificationService notificationService) : ICommandHandler<PushNotificationsChangeCommand>
{
    public async Task Handle(
        PushNotificationsChangeCommand request,
        CancellationToken cancellationToken)
    {
        var change = new NotificationsChangedResponse(
            request.Change,
            request.NotificationIds,
            request.AffectedCount,
            request.UnreadCount,
            request.OccurredAt);

        await notificationService.PushChangeAsync(request.RecipientId, change, cancellationToken);
    }
}
