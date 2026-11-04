using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when an avatar violates its rules.
/// </summary>
public sealed class InvalidAvatarException(string message)
    : DomainException(message);