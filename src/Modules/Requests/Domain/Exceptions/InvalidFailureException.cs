using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when failure parameters are invalid.
/// </summary>
public sealed class InvalidFailureException(string message)
    : DomainException(message);