using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when dispatch policy parameters are invalid.
/// </summary>
public sealed class InvalidPolicyException(string message)
    : DomainException(message);