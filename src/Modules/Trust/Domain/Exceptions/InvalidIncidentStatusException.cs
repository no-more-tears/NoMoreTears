using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when an action is not allowed in the current status of an incident.
/// </summary>
public sealed class InvalidIncidentStatusException(IncidentStatus status, string action)
    : DomainException($"Cannot {action} an incident in status '{status}'.");