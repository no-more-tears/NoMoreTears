using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Redis;

/// <summary>
///     Registers a Redis connectivity health check probe
///     with latency threshold awareness.
/// </summary>
public sealed class RedisHealthCheckContributor(
    RedisHealthCheckOptions options) : IHealthCheckContributor
{
    /// <inheritdoc />
    public void Register(IHealthChecksBuilder builder)
    {
        builder.Add(new HealthCheckRegistration(
            name: options.Name,
            factory: _ => new RedisHealthCheck(options),
            failureStatus: options.FailureStatus,
            tags: options.Tags));
    }
}