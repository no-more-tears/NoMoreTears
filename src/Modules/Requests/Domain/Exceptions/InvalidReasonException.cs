using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a reason given by a participant violates its rules.
/// </summary>
public sealed class InvalidReasonException(string message)
    : DomainException(message);