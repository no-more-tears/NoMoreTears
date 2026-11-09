namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationsAsRead;

public sealed class MarkNotificationsAsReadCommandHandler(
    INotificationService notificationService,
    IEventPublisher<INotificationOutbox> eventPublisher) : ICommandHandler<MarkNotificationsAsReadCommand, NotificationsChangedResponse>
{
    public async Task<NotificationsChangedResponse> Handle(
        MarkNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var change = await notificationService.MarkManyAsReadAsync(request.RecipientId, request.NotificationIds, cancellationToken);

        if (change.AffectedCount > 0)
        {
            await eventPublisher.PublishAsync(NotificationEvents.Changed(request.RecipientId, change), cancellationToken);
        }
        return change;
    }
}
