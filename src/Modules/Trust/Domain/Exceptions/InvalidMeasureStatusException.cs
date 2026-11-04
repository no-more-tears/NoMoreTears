using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when an action is not allowed in the current status of a temporary measure.
/// </summary>
public sealed class InvalidMeasureStatusException(MeasureStatus status, string action)
    : DomainException($"Cannot {action} a measure in status '{status}'.");