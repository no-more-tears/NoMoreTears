namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotificationsChange;

/// <summary>
///     Pushes a change made by the recipient to the other connected clients.
/// </summary>
public sealed record PushNotificationsChangeCommand(
    Guid RecipientId,
    string Change,
    IReadOnlyList<Guid> NotificationIds,
    int AffectedCount,
    long UnreadCount,
    DateTimeOffset OccurredAt) : ICommand;
