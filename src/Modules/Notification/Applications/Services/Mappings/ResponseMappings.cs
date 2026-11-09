using NotificationModel = NoMoreTears.Modules.Notification.Domain.Aggregates.Notification;
using NoMoreTears.Modules.Notification.Contracts.Responses;

namespace NoMoreTears.Modules.Notification.Applications.Services.Mappings;

/// <summary>
///     Manual mapping of domain models to responses.
/// </summary>
public static class ResponseMappings
{
    public static NotificationResponse ToResponse(this NotificationModel notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        return new NotificationResponse(
            notification.Id,
            notification.Type,
            notification.Category.ToString(),
            notification.Severity.ToString(),
            notification.Title,
            notification.Body,
            notification.Data,
            notification.Link,
            notification.OccurredAt,
            notification.CreatedAt,
            notification.DeliveredAt,
            notification.ReadAt,
            notification.IsRead);
    }
}