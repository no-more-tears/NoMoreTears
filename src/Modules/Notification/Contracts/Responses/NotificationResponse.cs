namespace NoMoreTears.Modules.Notification.Contracts.Responses;

/// <summary>
///     In-app notification of the current user.
/// </summary>
public sealed record NotificationResponse(
    Guid Id,
    string Type,
    string Category,
    string Severity,
    string Title,
    string Body,
    IReadOnlyDictionary<string, string> Data,
    string? Link,
    DateTimeOffset OccurredAt,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? DeliveredAt,
    DateTimeOffset? ReadAt,
    bool IsRead);