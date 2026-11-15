using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Trust.Contracts.Events;

/// <summary>
///     An incident was opened (FR-TRS-04). An accepted complaint about a request moves the request to dispute in Requests.
/// </summary>
/// <param name="Kind">One of <see cref="Constants.IncidentKinds"/>.</param>
/// <param name="RequestId">Request the incident is about; <c>null</c> for incidents about a party only.</param>
/// <param name="SubjectId">User identifier of the party the incident is about.</param>
/// <param name="SubjectRole">One of <see cref="Constants.PartyRoles"/>.</param>
public sealed record IncidentOpenedEvent(
    Guid IncidentId,
    string Kind,
    Guid? RequestId,
    Guid SubjectId,
    string SubjectRole,
    DateTimeOffset OpenedAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);