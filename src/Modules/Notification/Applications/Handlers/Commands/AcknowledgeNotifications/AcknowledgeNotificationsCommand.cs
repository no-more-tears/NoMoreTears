namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.AcknowledgeNotifications;

/// <summary>
///     Confirms that a client of the recipient received notifications in real time. Sent by the hub.
/// </summary>
public sealed record AcknowledgeNotificationsCommand(
    Guid RecipientId,
    IReadOnlyList<Guid> NotificationIds) : ICommand<int>;
