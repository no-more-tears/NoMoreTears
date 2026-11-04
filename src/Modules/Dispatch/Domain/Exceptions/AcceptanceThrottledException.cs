using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a throttled master accepts requests too often.
/// </summary>
public sealed class AcceptanceThrottledException(DateTimeOffset nextAllowedAt)
    : DomainException($"The next acceptance is allowed from {nextAllowedAt:O}.");