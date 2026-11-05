using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a frozen review is released by another incident.
/// </summary>
public sealed class IncidentMismatchException(Guid incidentId)
    : DomainException($"Incident '{incidentId}' is not the incident that froze the review.");