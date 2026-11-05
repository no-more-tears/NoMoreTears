using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a time-based transition of a measure is attempted too early.
/// </summary>
public sealed class MeasureNotDueException(string message)
    : DomainException(message);