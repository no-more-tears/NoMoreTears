using NoMoreTears.Modules.Notification.Contracts.Responses;

namespace NoMoreTears.Modules.Notification.Applications.Services.Abstractions;

/// <summary>
///     Pushes notifications to the connected clients of a user. Implemented by the SignalR host;
///     hosts without real-time connections use a no-op implementation.
/// </summary>
public interface IRealtimeNotifier
{
    Task SendNotificationAsync(
        Guid recipientId,
        NotificationResponse notification,
        CancellationToken cancellationToken);

    Task SendNotificationsChangedAsync(
        Guid recipientId,
        NotificationsChangedResponse change,
        CancellationToken cancellationToken);

    Task SendUnreadCountAsync(
        Guid recipientId,
        UnreadCountResponse unreadCount,
        CancellationToken cancellationToken);
}