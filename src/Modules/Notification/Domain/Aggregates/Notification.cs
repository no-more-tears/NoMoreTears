using NoMoreTears.Modules.Notification.Domain.Enums;
using NoMoreTears.Modules.Notification.Domain.Exceptions;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Notification.Domain.Aggregates;

/// <summary>
///     An in-app notification of a user.
/// </summary>
public sealed class Notification : AuditableEntity<Guid>
{
    /// <summary>
    ///     User the notification is addressed to.
    /// </summary>
    public Guid RecipientId { get; private set; }

    /// <summary>
    ///     Event of another module that caused the notification; one event creates at most one notification per user.
    /// </summary>
    public Guid SourceEventId { get; private set; }

    /// <summary>
    ///     Stable code of the kind of notification, for example <c>masters.verification.verified</c>;
    ///     the client may render its own text by it.
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    ///     Area of the platform the notification is about.
    /// </summary>
    public NotificationCategory Category { get; private set; }

    /// <summary>
    ///     How important the notification is.
    /// </summary>
    public NotificationSeverity Severity { get; private set; }

    /// <summary>
    ///     Short text shown in the list.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    ///     Full text.
    /// </summary>
    public string Body { get; private set; }

    /// <summary>
    ///     Identifiers and values the client needs to act on the notification (for example <c>masterId</c>, <c>reason</c>).
    /// </summary>
    public IReadOnlyDictionary<string, string> Data { get; private set; }

    /// <summary>
    ///     Relative route of the client that opens the subject of the notification.
    /// </summary>
    public string? Link { get; private set; }

    /// <summary>
    ///     Moment the fact described by the notification happened.
    /// </summary>
    public DateTimeOffset OccurredAt { get; private set; }

    /// <summary>
    ///     Moment a client of the recipient confirmed receiving the notification in real time.
    /// </summary>
    public DateTimeOffset? DeliveredAt { get; private set; }

    /// <summary>
    ///     Moment the recipient read the notification.
    /// </summary>
    public DateTimeOffset? ReadAt { get; private set; }

    /// <summary>
    ///     Indicates whether the recipient read the notification.
    /// </summary>
    public bool IsRead => ReadAt is not null;

    /// <summary>
    ///     Used only by persistence to materialize the notification.
    /// </summary>
    private Notification()
    {
        Type = null!;
        Title = null!;
        Body = null!;
        Data = null!;
    }

    private Notification(
        Guid id,
        Guid recipientId,
        Guid sourceEventId,
        string type,
        NotificationCategory category,
        NotificationSeverity severity,
        string title,
        string body,
        IReadOnlyDictionary<string, string> data,
        string? link,
        DateTimeOffset occurredAt)
    {
        Id = id;
        RecipientId = recipientId;
        SourceEventId = sourceEventId;
        Type = type;
        Category = category;
        Severity = severity;
        Title = title;
        Body = body;
        Data = data;
        Link = link;
        OccurredAt = occurredAt;
    }

    /// <summary>
    ///     Creates a notification for a user.
    /// </summary>
    public static Notification Create(
        Guid recipientId,
        Guid sourceEventId,
        string type,
        NotificationCategory category,
        NotificationSeverity severity,
        string title,
        string body,
        IReadOnlyDictionary<string, string> data,
        string? link,
        DateTimeOffset occurredAt,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (recipientId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(recipientId));
        }

        if (sourceEventId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(sourceEventId));
        }

        var notification = new Notification(
            Guid.CreateVersion7(createdAt),
            recipientId,
            sourceEventId,
            Require(type, "Type", NotificationRules.MaxTypeLength),
            category,
            severity,
            Require(title, "Title", NotificationRules.MaxTitleLength),
            Require(body, "Body", NotificationRules.MaxBodyLength),
            NormalizeData(data),
            NormalizeLink(link),
            occurredAt);

        notification.MarkAsCreated(createdAt);
        return notification;
    }

    /// <summary>
    ///     Records that a client of the recipient received the notification. Repeated confirmations keep the first moment.
    /// </summary>
    public void MarkAsDelivered(DateTimeOffset deliveredAt)
    {
        if (DeliveredAt is not null)
        {
            return;
        }

        DeliveredAt = deliveredAt;
        MarkAsUpdated(deliveredAt);
    }

    /// <summary>
    ///     Records that the recipient read the notification; a read notification is also delivered.
    ///     Reading again keeps the first moment.
    /// </summary>
    public void MarkAsRead(DateTimeOffset readAt)
    {
        if (ReadAt is not null)
        {
            return;
        }

        DeliveredAt ??= readAt;
        ReadAt = readAt;
        MarkAsUpdated(readAt);
    }

    private static string Require(string value, string name, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0 || normalized.Length > maxLength)
        {
            throw new InvalidNotificationException($"{name} must contain from 1 to {maxLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeLink(string? link)
    {
        var normalized = link?.Trim();

        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        if (normalized.Length > NotificationRules.MaxLinkLength || !normalized.StartsWith('/'))
        {
            throw new InvalidNotificationException(
                $"Link must be a relative route of at most {NotificationRules.MaxLinkLength} characters.");
        }

        return normalized;
    }

    private static Dictionary<string, string> NormalizeData(IReadOnlyDictionary<string, string> data)
    {
        if (data.Count > NotificationRules.MaxDataEntries)
        {
            throw new InvalidNotificationException(
                $"Notification data must contain at most {NotificationRules.MaxDataEntries} entries.");
        }

        var normalized = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var (key, value) in data)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length > NotificationRules.MaxDataKeyLength)
            {
                throw new InvalidNotificationException(
                    $"Notification data keys must contain from 1 to {NotificationRules.MaxDataKeyLength} characters.");
            }

            if (value is null || value.Length > NotificationRules.MaxDataValueLength)
            {
                throw new InvalidNotificationException(
                    $"Notification data values must contain at most {NotificationRules.MaxDataValueLength} characters.");
            }

            normalized[key] = value;
        }

        return normalized;
    }
}