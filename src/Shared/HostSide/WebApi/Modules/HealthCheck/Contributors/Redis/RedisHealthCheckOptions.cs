using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Redis;

/// <summary>
///     Configuration options for the Redis health check probe.
/// </summary>
public sealed class RedisHealthCheckOptions
{
    /// <summary>
    ///     Display name of the probe shown in health check responses.
    /// </summary>
    public string Name { get; init; } = "redis";

    /// <summary>
    ///     Redis connection string used by the probe.
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    ///     Password used to authenticate with the Redis server.
    ///     Set to <see langword="null"/> if the server does not require authentication.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    ///     Maximum acceptable round-trip time for a Redis PING command in milliseconds.
    ///     If exceeded, the probe reports <see cref="HealthStatus.Degraded"/> regardless of connectivity.
    ///     Set to <see langword="null"/> to disable latency threshold checking.
    ///     Defaults to 100 ms.
    /// </summary>
    public int? DegradedThresholdMs { get; init; } = 100;

    /// <summary>
    ///     Health status reported when the probe fails.
    ///     Use <see cref="HealthStatus.Degraded"/> to signal Redis is unreachable.
    ///     Defaults to <see cref="HealthStatus.Unhealthy"/>.
    /// </summary>
    public HealthStatus FailureStatus { get; init; } = HealthStatus.Unhealthy;

    /// <summary>
    ///     Tags associated with this probe.
    /// </summary>
    public string[] Tags { get; init; } = [HealthCheckTag.Readiness];
}