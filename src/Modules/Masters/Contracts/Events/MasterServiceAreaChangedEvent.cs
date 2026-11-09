using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Masters.Contracts.Events;

/// <summary>
///     The master changed the service area; a changed area awaits review again.
/// </summary>
public sealed record MasterServiceAreaChangedEvent(
    Guid MasterId,
    ServiceAreaResponse ServiceArea,
    string ServiceAreaStatus,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
