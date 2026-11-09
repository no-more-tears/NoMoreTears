namespace NoMoreTears.Modules.Notification.Domain.Enums;

/// <summary>
///     Area of the platform a notification is about; the client groups and filters notifications by it.
/// </summary>
public enum NotificationCategory
{
    Account,
    Verification,
    Profile,
    Sanction,
    Request,
    Dispute,
    System
}