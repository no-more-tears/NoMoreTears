using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     An archived category was activated again.
/// </summary>
public sealed record CategoryActivatedEvent(
    Guid CategoryId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
