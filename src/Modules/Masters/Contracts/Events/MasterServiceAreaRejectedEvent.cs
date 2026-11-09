using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     A reviewer rejected the service area of the master.
/// </summary>
public sealed record MasterServiceAreaRejectedEvent(
    Guid MasterId,
    Guid ReviewerId,
    string Reason,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
