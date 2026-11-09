namespace NoMoreTears.Modules.Notification.Contracts.Responses;

/// <summary>
///     Notifications of the current user changed (read or deleted).
/// </summary>
public sealed record NotificationsChangedResponse(
    string Change,
    IReadOnlyList<Guid> NotificationIds,
    int AffectedCount,
    long UnreadCount,
    DateTimeOffset OccurredAt);