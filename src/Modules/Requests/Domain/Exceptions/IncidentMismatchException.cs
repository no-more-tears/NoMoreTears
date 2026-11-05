using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a dispute is settled with an incident that is not open for the request.
/// </summary>
public sealed class IncidentMismatchException(Guid incidentId)
    : DomainException($"Incident '{incidentId}' is not the open incident of the request.");