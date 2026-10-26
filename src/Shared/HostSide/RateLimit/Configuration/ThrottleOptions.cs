namespace NoMoreTears.Shared.HostSide.RateLimit.Configuration;

/// <summary>
///     Configuration options for throttling.
/// </summary>
public sealed class ThrottleOptions
{
    public const string SectionName = "Throttle";

    /// <summary>
    ///     Maximum number of requests allowed per second. 
    /// </summary>
    public int RequestsPerSecond { get; init; } = 10;

    /// <summary>
    ///     How many seconds of burst traffic to allow.
    ///     BucketCapacity = RequestsPerSecond * BurstSeconds.
    ///     Example: 10 RPS * 3s = 30 token bucket, allowing a brief spike before throttling.
    /// </summary>
    public double BurstSeconds { get; init; } = 3.0;

    /// <summary>Http status code returned when the bucket is empty. </summary>
    public int StatusCode { get; init; } = 429;

    /// <summary>Whether to add Retry-After header indicating how long to wait. </summary>
    public bool AddRetryAfterHeader { get; init; } = true;

    /// <summary>
    ///     Whether to allow requests when the throttle storage is unavailable.
    ///     When disabled, storage failures surface as server errors.
    /// </summary>
    public bool FailOpen { get; init; } = true;
}