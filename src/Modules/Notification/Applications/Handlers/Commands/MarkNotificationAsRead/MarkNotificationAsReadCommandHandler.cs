namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler(
    INotificationService notificationService,
    IEventPublisher<INotificationOutbox> eventPublisher) : ICommandHandler<MarkNotificationAsReadCommand, NotificationResponse>
{
    public async Task<NotificationResponse> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var result = await notificationService.MarkAsReadAsync(request.RecipientId, request.NotificationId, cancellationToken);
        var change = result.Change;

        if (change.AffectedCount > 0)
        {
            await eventPublisher.PublishAsync(NotificationEvents.Changed(request.RecipientId, change), cancellationToken);
        }
        return result.Notification;
    }
}
