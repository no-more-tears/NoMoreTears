using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when an action is performed by someone who is not the expected participant of the request.
/// </summary>
public sealed class RequestParticipantMismatchException(string message)
    : DomainException(message);