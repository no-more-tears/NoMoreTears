using NoMoreTears.Modules.Dispatch.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when an action is not allowed in the current status of a distribution.
/// </summary>
public sealed class InvalidDistributionStatusException(DistributionStatus status, string action)
    : DomainException($"Cannot {action} a distribution in status '{status}'.");