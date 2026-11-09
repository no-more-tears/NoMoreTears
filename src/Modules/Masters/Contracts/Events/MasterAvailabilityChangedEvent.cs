using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The master switched availability on or off; Dispatch uses it to select candidates.
/// </summary>
public sealed record MasterAvailabilityChangedEvent(
    Guid MasterId,
    bool IsAvailable,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
