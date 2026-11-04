using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a service area or its coordinates violate their rules.
/// </summary>
public sealed class InvalidServiceAreaException(string message)
    : DomainException(message);