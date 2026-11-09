using NoMoreTears.Modules.Notification.Contracts.Responses;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Hubs;

/// <summary>
///     Methods the server calls on clients; names match <c>NotificationHub</c> constants of the contracts.
/// </summary>
public interface INotificationsClient
{
    Task ReceiveNotification(NotificationResponse notification);

    Task ReceiveNotifications(IReadOnlyList<NotificationResponse> notifications);

    Task NotificationsChanged(NotificationsChangedResponse change);

    Task UnreadCountChanged(UnreadCountResponse unreadCount);
}