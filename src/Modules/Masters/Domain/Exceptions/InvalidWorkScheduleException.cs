using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a work schedule violates its rules.
/// </summary>
public sealed class InvalidWorkScheduleException(string message)
    : DomainException(message);