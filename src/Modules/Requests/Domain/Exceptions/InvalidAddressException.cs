using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when an address or its coordinates violate their rules.
/// </summary>
public sealed class InvalidAddressException(string message)
    : DomainException(message);