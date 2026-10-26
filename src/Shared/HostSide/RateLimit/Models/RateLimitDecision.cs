namespace NoMoreTears.Shared.HostSide.RateLimit.Models;

/// <summary>
///     Result of a rate limit check.
/// </summary>
public sealed record RateLimitDecision(
    bool IsAllowed,
    int CurrentCount,
    int MaxRequests,
    int Remaining,
    DateTimeOffset ResetsAt)
{
    /// <summary>
    ///     Creates a new instance of <see cref="RateLimitDecision"/> representing an allowed request.
    /// </summary>
    public static RateLimitDecision Allowed(int currentCount, int maxRequests, DateTimeOffset resetAt) =>
        new(true, currentCount, maxRequests, Math.Max(0, maxRequests - currentCount), resetAt);

    /// <summary>
    ///     Creates a new instance of <see cref="RateLimitDecision"/> representing a blocked request.
    /// </summary>
    public static RateLimitDecision Blocked(int currentCount, int maxRequests, DateTimeOffset resetAt) =>
        new(false, currentCount, maxRequests, 0, resetAt);
}