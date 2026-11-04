using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a time-based transition is attempted before its deadline.
/// </summary>
public sealed class RequestDeadlineNotReachedException(string message)
    : DomainException(message);