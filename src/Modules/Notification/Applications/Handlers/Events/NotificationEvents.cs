namespace NoMoreTears.Modules.Notification.Applications.Handlers.Events;

/// <summary>
///     Builds internal events of the Notification module.
/// </summary>
internal static class NotificationEvents
{
    public static NotificationsChangedEvent Changed(Guid recipientId, NotificationsChangedResponse change)
    {
        ArgumentNullException.ThrowIfNull(change);

        return new NotificationsChangedEvent(
            recipientId,
            change.Change,
            change.NotificationIds,
            change.AffectedCount,
            change.UnreadCount,
            change.OccurredAt);
    }
}
