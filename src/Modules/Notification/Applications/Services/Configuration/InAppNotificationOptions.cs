namespace NoMoreTears.Modules.Notification.Applications.Services.Configuration;

/// <summary>
///     Options of in-app notifications.
/// </summary>
public sealed class InAppNotificationOptions
{
    public const string SectionName = "InApp";

    /// <summary>
    ///     Culture of the stored title and body (<c>uk</c> or <c>en</c>); clients may render their own text by the type.
    /// </summary>
    public string Culture { get; set; } = "uk";

    /// <summary>
    ///     Maximum number of not confirmed notifications sent to a client after it connects.
    /// </summary>
    public int UndeliveredBatchSize { get; set; } = 100;
}