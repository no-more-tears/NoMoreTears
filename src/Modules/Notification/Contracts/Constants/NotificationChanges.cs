namespace NoMoreTears.Modules.Notification.Contracts.Constants;

/// <summary>
///     Kinds of changes of notifications made by the recipient.
/// </summary>
public static class NotificationChanges
{
    public const string Read = nameof(Read);

    public const string ReadAll = nameof(ReadAll);

    public const string Deleted = nameof(Deleted);
}