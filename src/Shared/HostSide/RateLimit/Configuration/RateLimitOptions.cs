namespace NoMoreTears.Shared.HostSide.RateLimit.Configuration;

/// <summary>
///     Configuration options for rate limiting.
/// </summary>
public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    /// <summary>
    ///     Maximum number of requests allowed within the window. 
    /// </summary>
    public int MaxRequests { get; init; } = 300;
    
    /// <summary>
    ///     Size of the rate limiting window in seconds. 
    /// </summary>
    public int WindowSizeInSeconds { get; init; } = 60;

    /// <summary>
    ///     Http status code to return when the rate limit is exceeded. 
    /// </summary>
    public int StatusCode { get; init; } = 429;

    /// <summary>
    ///     Whether to include rate limit headers in the response.
    /// </summary>
    public bool AddRateLimitHeaders { get; init; } = true;

    /// <summary>
    ///     Whether to allow requests when the rate limit storage is unavailable.
    ///     When disabled, storage failures surface as server errors.
    /// </summary>
    public bool FailOpen { get; init; } = true;
}