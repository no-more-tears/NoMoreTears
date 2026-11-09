namespace NoMoreTears.Modules.Notification.Contracts.Requests;

/// <summary>
///     Marks several notifications of the current user as read.
/// </summary>
public sealed record MarkNotificationsAsReadRequest(
    IReadOnlyList<Guid> NotificationIds);