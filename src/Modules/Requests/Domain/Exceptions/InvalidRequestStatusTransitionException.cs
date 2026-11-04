using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when an action is not allowed by the request state machine in the current status.
/// </summary>
public sealed class InvalidRequestStatusTransitionException(RequestStatus status, string action)
    : DomainException($"Cannot {action} a request in status '{status}'.");