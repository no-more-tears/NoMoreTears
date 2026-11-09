namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.DeleteNotification;

public sealed record DeleteNotificationCommand(
    Guid RecipientId,
    Guid NotificationId) : ICommand<NotificationsChangedResponse>;
