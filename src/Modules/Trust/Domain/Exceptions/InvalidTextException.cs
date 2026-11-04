using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a statement, justification, explanation or comment violates its rules.
/// </summary>
public sealed class InvalidTextException(string message)
    : DomainException(message);