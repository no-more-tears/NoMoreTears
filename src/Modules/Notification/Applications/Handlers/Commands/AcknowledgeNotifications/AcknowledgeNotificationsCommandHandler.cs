namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.AcknowledgeNotifications;

public sealed class AcknowledgeNotificationsCommandHandler(
    INotificationService notificationService) : ICommandHandler<AcknowledgeNotificationsCommand, int>
{
    public async Task<int> Handle(
        AcknowledgeNotificationsCommand request,
        CancellationToken cancellationToken)
    {
        return await notificationService.AcknowledgeAsync(request.RecipientId, request.NotificationIds, cancellationToken);
    }
}
