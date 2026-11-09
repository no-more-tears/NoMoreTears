namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;

public sealed class CreateNotificationCommandHandler(
    INotificationService notificationService,
    IEventPublisher<INotificationOutbox> eventPublisher) : ICommandHandler<CreateNotificationCommand>
{
    public async Task Handle(
        CreateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await notificationService.CreateAsync(
            request.RecipientId,
            request.SourceEventId,
            request.Type,
            request.Data,
            request.OccurredAt,
            cancellationToken);

        // Pushed by another consumer once the notification is committed.
        if (notification is not null)
        {
            await eventPublisher.PublishAsync(
                new NotificationCreatedEvent(notification.Id, request.RecipientId, notification.CreatedAt!.Value),
                cancellationToken);
        }
    }
}
