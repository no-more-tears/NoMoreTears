using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A reviewer confirmed the service area of the master.
/// </summary>
public sealed record MasterServiceAreaVerifiedEvent(
    Guid MasterId,
    Guid ReviewerId,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
