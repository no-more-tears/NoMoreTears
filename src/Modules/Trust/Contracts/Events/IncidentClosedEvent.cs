using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Trust.Contracts.Events;

/// <summary>
///     An incident was closed after the decision or the appeal; a disputed request returns to its previous status.
/// </summary>
/// <param name="RequestId">Request the incident is about; <c>null</c> for incidents about a party only.</param>
public sealed record IncidentClosedEvent(
    Guid IncidentId,
    Guid? RequestId,
    DateTimeOffset ClosedAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);