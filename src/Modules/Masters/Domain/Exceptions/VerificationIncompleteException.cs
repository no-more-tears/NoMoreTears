using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a master is verified before all verification conditions are met.
/// </summary>
public sealed class VerificationIncompleteException(string message)
    : DomainException(message);