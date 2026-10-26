using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.ExternalApi;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Postgres;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Redis;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Configuration;

/// <summary>
///     Configuration options for the health check web module.
/// </summary>
public sealed class HealthCheckOptions
{
    /// <summary>
    ///     Indicates whether the API liveness probe is enabled.
    /// </summary>
    internal bool ApiEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether the PostgreSQL readiness probe is enabled.
    /// </summary>
    internal bool PostgresEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether the Redis readiness probe is enabled.
    /// </summary>
    internal bool RedisEnabled { get; private set; }

    /// <summary>
    ///     Explicitly provided contributor instances registered alongside built-in probes.
    /// </summary>
    internal IHealthCheckContributor[] ExplicitContributors { get; private set; } = [];

    /// <summary>
    ///     Configuration options for the API liveness probe.
    /// </summary>
    internal ExternalApiHealthCheckOptions? ApiOptions { get; private set; }

    /// <summary>
    ///    Configuration options for the PostgreSQL readiness probe.
    /// </summary>
    internal PostgresHealthCheckOptions? PostgresOptions { get; private set; }

    /// <summary>
    ///    Configuration options for the Redis readiness probe.
    /// </summary>
    internal RedisHealthCheckOptions? RedisOptions { get; private set; }

    /// <summary>
    ///     Enables the API liveness probe with the specified options.
    /// </summary>
    public HealthCheckOptions UseApi(ExternalApiHealthCheckOptions options)
    {
        ApiOptions = options;
        ApiEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the API liveness probe with default options.
    /// </summary>
    public HealthCheckOptions UseApi()
    {
        return UseApi(new ExternalApiHealthCheckOptions());
    }

    /// <summary>
    ///     Enables the PostgreSQL readiness probe with the specified options.
    /// </summary>
    public HealthCheckOptions UsePostgres(PostgresHealthCheckOptions options)
    {
        PostgresOptions = options;
        PostgresEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables the Redis readiness probe with the specified options.
    /// </summary>
    public HealthCheckOptions UseRedis(RedisHealthCheckOptions options)
    {
        RedisOptions = options;
        RedisEnabled = true;
        return this;
    }

    /// <summary>
    ///     Includes explicitly provided contributor instances alongside built-in probes.
    ///     Use this to register custom <see cref="IHealthCheckContributor"/> implementations.
    /// </summary>
    public HealthCheckOptions WithContributors(params IHealthCheckContributor[] contributors)
    {
        ExplicitContributors = contributors;
        return this;
    }

    /// <summary>
    ///     Enables all built-in probes at once.
    /// </summary>
    public HealthCheckOptions UseAllProbes(
        PostgresHealthCheckOptions postgresOptions,
        RedisHealthCheckOptions redisOptions)
    {
        return UseApi()
            .UsePostgres(postgresOptions)
            .UseRedis(redisOptions);
    }
}