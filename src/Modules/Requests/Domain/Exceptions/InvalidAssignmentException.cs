using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when assignment parameters of a master are invalid.
/// </summary>
public sealed class InvalidAssignmentException(string message)
    : DomainException(message);