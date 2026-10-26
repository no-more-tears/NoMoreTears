using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.HostSide.RateLimit.Abstractions;
using NoMoreTears.Shared.HostSide.RateLimit.Configuration;
using NoMoreTears.Shared.HostSide.RateLimit.Models;
using StackExchange.Redis;
using System.Globalization;

namespace NoMoreTears.Shared.HostSide.RateLimit;

/// <summary>
///     Sliding window rate limiter implementation backed by Redis.
/// </summary>
public sealed class SlidingWindowRateLimiter(
    IConnectionMultiplexer redis,
    TimeProvider timeProvider,
    IOptions<RateLimitOptions> options,
    ILogger<SlidingWindowRateLimiter> logger) : IRateLimiter
{
    // Reads both windows and increments the current one in a single atomic step,
    // so concurrent requests cannot all observe the same count and slip through.
    // KEYS[1] = current window, KEYS[2] = previous window.
    // ARGV[1] = max requests, ARGV[2] = previous window weight, ARGV[3] = TTL in ms.
    private const string CheckScript = """
        local current = tonumber(redis.call('GET', KEYS[1]) or '0')
        local previous = tonumber(redis.call('GET', KEYS[2]) or '0')

        if current + previous * tonumber(ARGV[2]) >= tonumber(ARGV[1]) then
            return { 0, current, previous }
        end

        current = redis.call('INCR', KEYS[1])
        if current == 1 then
            redis.call('PEXPIRE', KEYS[1], ARGV[3])
        end

        return { 1, current, previous }
        """;

    private readonly RateLimitOptions _options = options.Value;

    /// <inheritdoc/>
    public async Task<RateLimitDecision> CheckAsync(
        string key,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var nowMs = timeProvider.GetUtcNow().ToUnixTimeMilliseconds();
        var windowMs = _options.WindowSizeInSeconds * 1000L;

        var currentWindow = nowMs / windowMs;
        var windowStartMs = currentWindow * windowMs;
        var windowEnd = DateTimeOffset.FromUnixTimeMilliseconds(windowStartMs + windowMs);

        // At the very start of a window -> weight ≈ 1.0 (previous window still fully counts).
        // At the very end of a window -> weight ≈ 0.0 (previous window is almost irrelevant).
        var weight = 1.0 - (double)(nowMs - windowStartMs) / windowMs;

        RedisResult[] result;

        try
        {
            // TTL = 2 windows so the previous window entry is still readable when we cross the boundary.
            result = (RedisResult[])(await redis.GetDatabase().ScriptEvaluateAsync(
                CheckScript,
                [BuildCacheKey(key, currentWindow), BuildCacheKey(key, currentWindow - 1)],
                [_options.MaxRequests, weight, windowMs * 2]))!;
        }
        catch (Exception exception) when (_options.FailOpen && exception is RedisException or TimeoutException)
        {
            logger.LogWarning(exception, "Rate limit storage is unavailable, the request is allowed without a check.");
            return RateLimitDecision.Allowed(0, _options.MaxRequests, windowEnd);
        }

        var isAllowed = (int)result[0] == 1;
        var current = (long)result[1];
        var previous = (long)result[2];

        if (!isAllowed)
        {
            var retryAt = ComputeRetryAt(current, previous, windowStartMs, windowMs);
            return RateLimitDecision.Blocked((int)current, _options.MaxRequests, retryAt);
        }

        return RateLimitDecision.Allowed(
            (int)Math.Ceiling(current + previous * weight),
            _options.MaxRequests,
            windowEnd);
    }

    /// <summary>
    ///     Moment when the weighted count drops below the limit again.
    /// </summary>
    private DateTimeOffset ComputeRetryAt(
        long current,
        long previous,
        long windowStartMs,
        long windowMs)
    {
        var max = _options.MaxRequests;

        if (current < max)
        {
            // Still within the current window: current + previous * w < max  =>  w < (max - current) / previous.
            var neededWeight = (double)(max - current) / previous;

            return DateTimeOffset.FromUnixTimeMilliseconds(
                windowStartMs + GetElapsedUntilWeight(neededWeight, windowMs));
        }

        // The current window is exhausted and becomes the previous one in the next window:
        // 0 + current * w < max  =>  w < max / current.
        var nextWindowWeight = (double)max / current;

        return DateTimeOffset.FromUnixTimeMilliseconds(
            windowStartMs + windowMs + GetElapsedUntilWeight(nextWindowWeight, windowMs));
    }

    /// <summary>
    ///     Milliseconds since the window start after which the previous window weight drops below <paramref name="weight"/>.
    /// </summary>
    private static long GetElapsedUntilWeight(double weight, long windowMs) =>
        (long)Math.Ceiling(windowMs * (1.0 - weight)) + 1;

    /// <summary>
    ///     The limit parameters are part of the key, so endpoints with different limits never share counters.
    ///     The braces form a Redis hash tag, keeping both windows in the same cluster slot.
    /// </summary>
    private string BuildCacheKey(string key, long window) =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"rl:{{{key}}}:{_options.MaxRequests}:{_options.WindowSizeInSeconds}:{window}");
}