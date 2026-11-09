namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUndeliveredNotifications;

/// <summary>
///     Returns notifications that no client of the recipient confirmed yet. Sent by the hub after a client connects.
/// </summary>
public sealed record GetUndeliveredNotificationsQuery(
    Guid RecipientId) : IQuery<IReadOnlyList<NotificationResponse>>;
