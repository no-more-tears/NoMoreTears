using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Postgres;

/// <summary>
///     Configuration options for the PostgreSQL health check probe.
/// </summary>
public sealed class PostgresHealthCheckOptions
{
    /// <summary>
    ///     Display name of the probe shown in health check responses.
    /// </summary>
    public string Name { get; init; } = "postgres";

    /// <summary>
    ///     PostgreSQL connection string used by the probe.
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    ///     Maximum time in seconds to wait for the health query to complete
    ///     before considering the probe failed. Defaults to 5 seconds.
    /// </summary>
    public int CommandTimeout { get; init; } = 5;

    /// <summary>
    ///     Maximum acceptable duration for the health query in milliseconds.
    ///     If exceeded, the probe reports <see cref="HealthStatus.Degraded"/> regardless of query success.
    ///     Set to <see langword="null"/> to disable latency threshold checking.
    ///     Defaults to 500 ms.
    /// </summary>
    public int? DegradedThresholdMs { get; init; } = 500;

    /// <summary>
    ///     Health status reported when the probe fails.
    ///     Use <see cref="HealthStatus.Degraded"/> to signal the database is unreachable.
    ///     Defaults to <see cref="HealthStatus.Unhealthy"/>.
    /// </summary>
    public HealthStatus FailureStatus { get; init; } = HealthStatus.Unhealthy;

    /// <summary>
    ///     Tags associated with this probe.
    /// </summary>
    public string[] Tags { get; init; } = [HealthCheckTag.Readiness];
}