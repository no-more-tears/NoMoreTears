namespace NoMoreTears.Shared.HostSide.RateLimit.Abstractions;

/// <summary>
///     Token bucket throttler for controlling the rate of requests while allowing for bursts.
/// </summary>
public interface IThrottler
{
    /// <summary>
    ///     Tries to consume a token for the request associated with the given key.
    /// </summary>
    Task<double?> TryConsumeAsync(string key, CancellationToken cancellationToken);
}