using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Notification.Contracts.Events;

/// <summary>
///     Internal event of the Notification module: a notification was stored and has to be pushed to the connected
///     clients of the recipient.
/// </summary>
public sealed record NotificationCreatedEvent(
    Guid NotificationId,
    Guid RecipientId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);