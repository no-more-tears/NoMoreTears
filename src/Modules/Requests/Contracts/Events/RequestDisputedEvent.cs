using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     Trust accepted a complaint: the request is disputed until the incident is closed (FR-TRS-04).
/// </summary>
public sealed record RequestDisputedEvent(
    Guid RequestId,
    Guid ClientId,
    Guid? MasterId,
    Guid IncidentId,
    string StatusBeforeDispute,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);