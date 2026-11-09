namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand(
    Guid RecipientId) : ICommand<NotificationsChangedResponse>;
