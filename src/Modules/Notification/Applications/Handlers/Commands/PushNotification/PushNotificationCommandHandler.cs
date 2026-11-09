namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotification;

public sealed class PushNotificationCommandHandler(
    INotificationService notificationService) : ICommandHandler<PushNotificationCommand>
{
    public async Task Handle(
        PushNotificationCommand request,
        CancellationToken cancellationToken)
    {
        await notificationService.PushAsync(request.NotificationId, cancellationToken);
    }
}
