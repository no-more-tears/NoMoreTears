using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a publication restriction violates its rules.
/// </summary>
public sealed class InvalidRestrictionException(string message)
    : DomainException(message);