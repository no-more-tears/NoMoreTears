using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when publication policy parameters are invalid.
/// </summary>
public sealed class InvalidPublicationPolicyException(string message)
    : DomainException(message);