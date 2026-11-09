namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(
    Guid RecipientId,
    Guid NotificationId) : ICommand<NotificationResponse>;
