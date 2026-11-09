using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Notification.Contracts.Events;

/// <summary>
///     Internal event of the Notification module: the recipient read or deleted notifications (for example over http),
///     so the other connected clients of the recipient must update their list and unread count.
/// </summary>
public sealed record NotificationsChangedEvent(
    Guid RecipientId,
    string Change,
    IReadOnlyList<Guid> NotificationIds,
    int AffectedCount,
    long UnreadCount,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);