using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.HostSide.RateLimit.Abstractions;
using NoMoreTears.Shared.HostSide.RateLimit.Configuration;
using StackExchange.Redis;
using System.Globalization;

namespace NoMoreTears.Shared.HostSide.RateLimit;

/// <summary>
///     Token bucket throttler implementation backed by Redis that allows for bursts of requests
///     while maintaining an average rate over time.
/// </summary>
public sealed class TokenBucketThrottler(
    IConnectionMultiplexer redis,
    TimeProvider timeProvider,
    IOptions<ThrottleOptions> options,
    ILogger<TokenBucketThrottler> logger) : IThrottler
{
    // Refills and consumes a token in a single atomic step.
    // KEYS[1] = bucket.
    // ARGV[1] = capacity, ARGV[2] = refill rate per ms, ARGV[3] = now in ms, ARGV[4] = TTL in ms.
    // Returns 0 when a token was consumed, otherwise the wait time in ms.
    private const string ConsumeScript = """
        local capacity = tonumber(ARGV[1])
        local rate = tonumber(ARGV[2])
        local now = tonumber(ARGV[3])

        local state = redis.call('HMGET', KEYS[1], 'tokens', 'ts')
        local tokens = tonumber(state[1]) or capacity
        local ts = tonumber(state[2]) or now

        -- Refill tokens based on time elapsed since last request.
        tokens = math.min(capacity, tokens + math.max(0, now - ts) * rate)

        if tokens < 1 then
            return math.ceil((1 - tokens) / rate)
        end

        redis.call('HSET', KEYS[1], 'tokens', tostring(tokens - 1), 'ts', tostring(now))
        redis.call('PEXPIRE', KEYS[1], ARGV[4])

        return 0
        """;

    private readonly ThrottleOptions _options = options.Value;
    private double _bucketCapacity => _options.RequestsPerSecond * _options.BurstSeconds;
    private double _refillRatePerMs => _options.RequestsPerSecond / 1000.0;

    /// <inheritdoc/>
    public async Task<double?> TryConsumeAsync(
        string key,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var nowMs = timeProvider.GetUtcNow().ToUnixTimeMilliseconds();

        // An expired bucket is equivalent to a full one, so keep it only for the time of a full refill.
        var ttlMs = (long)Math.Ceiling(_bucketCapacity / _refillRatePerMs) + 5000;

        long waitMs;

        try
        {
            waitMs = (long)await redis.GetDatabase().ScriptEvaluateAsync(
                ConsumeScript,
                [BuildCacheKey(key)],
                [_bucketCapacity, _refillRatePerMs, nowMs, ttlMs]);
        }
        catch (Exception exception) when (_options.FailOpen && exception is RedisException or TimeoutException)
        {
            logger.LogWarning(exception, "Throttle storage is unavailable, the request is allowed without a check.");
            return null;
        }

        return waitMs > 0 ? Math.Ceiling(waitMs / 1000.0) : null;
    }

    /// <summary>
    ///     The throttle parameters are part of the key, so endpoints with different limits never share buckets.
    /// </summary>
    private string BuildCacheKey(string key) =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"tb:{{{key}}}:{_options.RequestsPerSecond}:{_options.BurstSeconds}");
}