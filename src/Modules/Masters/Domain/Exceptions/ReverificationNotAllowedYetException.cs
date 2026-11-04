using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a master with revoked verification applies again before the waiting period ends.
/// </summary>
public sealed class ReverificationNotAllowedYetException(DateTimeOffset allowedFrom)
    : DomainException($"Repeated verification is allowed only from {allowedFrom:O}.");