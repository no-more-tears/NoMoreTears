namespace NoMoreTears.Modules.Notification.Contracts.Constants;

public static class EndpointName
{
    public const string GetNotifications = nameof(GetNotifications);

    public const string GetUnreadNotificationsCount = nameof(GetUnreadNotificationsCount);

    public const string MarkNotificationAsRead = nameof(MarkNotificationAsRead);

    public const string MarkNotificationsAsRead = nameof(MarkNotificationsAsRead);

    public const string MarkAllNotificationsAsRead = nameof(MarkAllNotificationsAsRead);

    public const string DeleteNotification = nameof(DeleteNotification);
}