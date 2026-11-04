using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when an action conflicts with the verification hold of a master.
/// </summary>
public sealed class VerificationHoldException(string message)
    : DomainException(message);