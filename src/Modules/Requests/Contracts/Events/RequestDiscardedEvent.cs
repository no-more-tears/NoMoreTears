using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     The client deleted a draft; it was never published.
/// </summary>
public sealed record RequestDiscardedEvent(
    Guid RequestId,
    Guid ClientId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);