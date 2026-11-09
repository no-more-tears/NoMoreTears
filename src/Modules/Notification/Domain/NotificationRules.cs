namespace NoMoreTears.Modules.Notification.Domain;

/// <summary>
///     Limits of notifications.
/// </summary>
public static class NotificationRules
{
    public const int MaxTypeLength = 100;

    public const int MaxTitleLength = 200;

    public const int MaxBodyLength = 2000;

    public const int MaxLinkLength = 500;

    public const int MaxDataEntries = 20;

    public const int MaxDataKeyLength = 50;

    public const int MaxDataValueLength = 500;

    public const int MaxTemplateLength = 100;

    public const int MaxErrorLength = 1000;
}