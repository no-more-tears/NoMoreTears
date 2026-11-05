using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a first or last name violates its rules.
/// </summary>
public sealed class InvalidFullNameException(string message)
    : DomainException(message);