using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a master exceeds the rate of acceptance attempts.
/// </summary>
public sealed class AcceptanceRateLimitExceededException(int limit)
    : DomainException($"At most {limit} acceptance attempts per minute are allowed.");