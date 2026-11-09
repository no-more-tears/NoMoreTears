namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandHandler(
    INotificationService notificationService,
    IEventPublisher<INotificationOutbox> eventPublisher) : ICommandHandler<MarkAllNotificationsAsReadCommand, NotificationsChangedResponse>
{
    public async Task<NotificationsChangedResponse> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var change = await notificationService.MarkAllAsReadAsync(request.RecipientId, cancellationToken);

        if (change.AffectedCount > 0)
        {
            await eventPublisher.PublishAsync(NotificationEvents.Changed(request.RecipientId, change), cancellationToken);
        }
        return change;
    }
}
