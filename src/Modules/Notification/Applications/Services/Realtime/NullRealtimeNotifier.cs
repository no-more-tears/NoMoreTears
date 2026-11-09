using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Contracts.Responses;

namespace NoMoreTears.Modules.Notification.Applications.Services.Realtime;

/// <summary>
///     Default <see cref="IRealtimeNotifier"/> of hosts without real-time connections (for example WebApi):
///     those hosts publish events and the SignalR host pushes them.
/// </summary>
public sealed class NullRealtimeNotifier : IRealtimeNotifier
{
    public Task SendNotificationAsync(
        Guid recipientId,
        NotificationResponse notification,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task SendNotificationsChangedAsync(
        Guid recipientId,
        NotificationsChangedResponse change,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task SendUnreadCountAsync(
        Guid recipientId,
        UnreadCountResponse unreadCount,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;
}