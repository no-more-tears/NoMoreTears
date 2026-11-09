namespace NoMoreTears.Modules.Notification.HostSide.Endpoints.Notifications;

public static class Routes
{
    public const string Base = "/notifications";

    public const string UnreadCount = $"{Base}/unread-count";

    public const string Notification = $"{Base}/{{notificationId:guid}}";

    public const string ReadOne = $"{Notification}/read";

    public const string ReadMany = $"{Base}/read";

    public const string ReadAll = $"{Base}/read-all";
}
