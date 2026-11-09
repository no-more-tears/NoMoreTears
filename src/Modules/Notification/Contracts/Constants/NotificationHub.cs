namespace NoMoreTears.Modules.Notification.Contracts.Constants;

/// <summary>
///     Contract of the real-time notification hub for clients.
/// </summary>
/// <remarks>
///     Connect to <see cref="Route"/> with the access token in the <c>access_token</c> query parameter.
///     The hub speaks the MessagePack and JSON protocols. After connecting, the client receives every notification
///     it has not confirmed yet (<see cref="ReceiveNotifications"/>) and the unread count; afterwards each new
///     notification arrives with <see cref="ReceiveNotification"/>. The client confirms received notifications with
///     <see cref="Acknowledge"/>, otherwise they are sent again on the next connection.
/// </remarks>
public static class NotificationHub
{
    /// <summary>
    ///     Route of the hub in the SignalR host.
    /// </summary>
    public const string Route = "/hubs/notifications";

    /// <summary>
    ///     Server to client: one new notification (<c>NotificationResponse</c>).
    /// </summary>
    public const string ReceiveNotification = nameof(ReceiveNotification);

    /// <summary>
    ///     Server to client: notifications not confirmed yet, sent after connecting (<c>NotificationResponse[]</c>).
    /// </summary>
    public const string ReceiveNotifications = nameof(ReceiveNotifications);

    /// <summary>
    ///     Server to client: notifications were read or deleted on another device (<c>NotificationsChangedResponse</c>).
    /// </summary>
    public const string NotificationsChanged = nameof(NotificationsChanged);

    /// <summary>
    ///     Server to client: current number of unread notifications (<c>UnreadCountResponse</c>).
    /// </summary>
    public const string UnreadCountChanged = nameof(UnreadCountChanged);

    /// <summary>
    ///     Client to server: confirms that notifications were received (<c>Guid[]</c>).
    /// </summary>
    public const string Acknowledge = nameof(Acknowledge);
}