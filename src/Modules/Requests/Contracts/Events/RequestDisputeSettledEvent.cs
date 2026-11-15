using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Requests.Contracts.Events;

/// <summary>
///     Trust closed the incident; the request returned to <see cref="Status"/>, the status it had before the dispute.
/// </summary>
public sealed record RequestDisputeSettledEvent(
    Guid RequestId,
    Guid ClientId,
    Guid? MasterId,
    Guid IncidentId,
    string Status,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);