namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.DeleteNotification;

public sealed class DeleteNotificationCommandHandler(
    INotificationService notificationService,
    IEventPublisher<INotificationOutbox> eventPublisher) : ICommandHandler<DeleteNotificationCommand, NotificationsChangedResponse>
{
    public async Task<NotificationsChangedResponse> Handle(
        DeleteNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var change = await notificationService.DeleteAsync(request.RecipientId, request.NotificationId, cancellationToken);

        if (change.AffectedCount > 0)
        {
            await eventPublisher.PublishAsync(NotificationEvents.Changed(request.RecipientId, change), cancellationToken);
        }
        return change;
    }
}
