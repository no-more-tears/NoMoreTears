using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a request is not among the active requests of the master.
/// </summary>
public sealed class ActiveAssignmentNotFoundException(Guid requestId)
    : DomainException($"Request '{requestId}' is not active for the master.");