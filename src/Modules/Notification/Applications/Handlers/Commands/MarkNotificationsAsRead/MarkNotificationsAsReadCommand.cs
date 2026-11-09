namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationsAsRead;

public sealed record MarkNotificationsAsReadCommand(
    Guid RecipientId,
    IReadOnlyList<Guid> NotificationIds) : ICommand<NotificationsChangedResponse>;
