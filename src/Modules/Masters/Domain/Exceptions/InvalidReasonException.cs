using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a reason violates its rules.
/// </summary>
public sealed class InvalidReasonException(string message)
    : DomainException(message);