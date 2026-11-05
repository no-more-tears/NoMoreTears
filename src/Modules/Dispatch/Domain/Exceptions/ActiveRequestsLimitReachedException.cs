using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when accepting would exceed the limit of active requests of a master.
/// </summary>
public sealed class ActiveRequestsLimitReachedException(int limit)
    : DomainException($"A master can have at most {limit} active requests.");