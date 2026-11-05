using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when an action is not allowed in the current verification status of a master.
/// </summary>
public sealed class InvalidVerificationStatusException(VerificationStatus status, string action)
    : DomainException($"Cannot {action} a master with verification status '{status}'.");