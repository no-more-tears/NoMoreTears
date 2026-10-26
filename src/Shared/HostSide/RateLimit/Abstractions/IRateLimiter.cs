using NoMoreTears.Shared.HostSide.RateLimit.Models;

namespace NoMoreTears.Shared.HostSide.RateLimit.Abstractions;

/// <summary>
///     Window rate limiter for controlling the number of requests allowed within a specified time window.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    ///     Checks if the request associated with the given key is allowed based on the rate limiting rules.
    /// </summary>
    Task<RateLimitDecision> CheckAsync(string key, CancellationToken cancellationToken);
}