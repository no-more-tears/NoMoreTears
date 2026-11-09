using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Contracts.Responses;
using NoMoreTears.Modules.Notification.HostSide.SignalR.Hubs;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Realtime;

/// <summary>
///     Pushes notifications through the hub context.
/// </summary>
public sealed class HubRealtimeNotifier(
    IHubContext<NotificationsHub,
        INotificationsClient> hubContext) : IRealtimeNotifier
{
    public Task SendNotificationAsync(Guid recipientId, NotificationResponse notification, CancellationToken cancellationToken) =>
        User(recipientId).ReceiveNotification(notification);

    public Task SendNotificationsChangedAsync(Guid recipientId, NotificationsChangedResponse change, CancellationToken cancellationToken) =>
        User(recipientId).NotificationsChanged(change);

    public Task SendUnreadCountAsync(Guid recipientId, UnreadCountResponse unreadCount, CancellationToken cancellationToken) =>
        User(recipientId).UnreadCountChanged(unreadCount);

    private INotificationsClient User(Guid recipientId) =>
        hubContext.Clients.User(recipientId.ToString("D", CultureInfo.InvariantCulture));
}