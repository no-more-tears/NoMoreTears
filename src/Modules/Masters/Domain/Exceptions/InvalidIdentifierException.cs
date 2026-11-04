using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a required identifier is empty.
/// </summary>
public sealed class InvalidIdentifierException(string name)
    : DomainException($"Identifier '{name}' must not be empty.");