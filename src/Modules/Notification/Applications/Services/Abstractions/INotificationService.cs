using NoMoreTears.Modules.Notification.Applications.Services.Results;
using NoMoreTears.Modules.Notification.Contracts.Responses;
using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Notification.Applications.Services.Abstractions;

/// <summary>
///     In-app notifications: storage, reading and real-time delivery.
/// </summary>
public interface INotificationService
{
    /// <summary>
    ///     Stores a notification caused by an event of another module;
    ///     returns <c>null</c> when the event already created a notification for the recipient.
    /// </summary>
    Task<NotificationResponse?> CreateAsync(
        Guid recipientId,
        Guid sourceEventId,
        string type,
        IReadOnlyDictionary<string, string> data,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken);

    Task<PagedResponse<NotificationResponse>> GetPageAsync(
        Guid recipientId,
        bool unreadOnly,
        string? category,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<UnreadCountResponse> GetUnreadCountAsync(
        Guid recipientId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns notifications that no client of the recipient confirmed yet, oldest first.
    /// </summary>
    Task<IReadOnlyList<NotificationResponse>> GetUndeliveredAsync(
        Guid recipientId,
        CancellationToken cancellationToken);

    Task<NotificationReadResult> MarkAsReadAsync(
        Guid recipientId,
        Guid notificationId,
        CancellationToken cancellationToken);

    Task<NotificationsChangedResponse> MarkManyAsReadAsync(
        Guid recipientId,
        IReadOnlyCollection<Guid> notificationIds,
        CancellationToken cancellationToken);

    Task<NotificationsChangedResponse> MarkAllAsReadAsync(
        Guid recipientId,
        CancellationToken cancellationToken);

    Task<NotificationsChangedResponse> DeleteAsync(
        Guid recipientId,
        Guid notificationId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Records that a client of the recipient received the notifications; returns how many were confirmed now.
    /// </summary>
    Task<int> AcknowledgeAsync(
        Guid recipientId,
        IReadOnlyCollection<Guid> notificationIds,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Pushes a stored notification and the new unread count to the connected clients of its recipient.
    /// </summary>
    Task PushAsync(
        Guid notificationId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Pushes a change made by the recipient to the other connected clients.
    /// </summary>
    Task PushChangeAsync(
        Guid recipientId,
        NotificationsChangedResponse change,
        CancellationToken cancellationToken);
}