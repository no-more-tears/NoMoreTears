using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Postgres;

/// <summary>
///     Registers a PostgreSQL connectivity health check.
/// </summary>
public sealed class PostgresHealthCheckContributor(
    PostgresHealthCheckOptions options) : IHealthCheckContributor
{
    /// <inheritdoc />
    public void Register(IHealthChecksBuilder builder)
    {
        builder.Add(new HealthCheckRegistration(
            name: options.Name,
            factory: _ => new PostgresHealthCheck(options),
            failureStatus: options.FailureStatus,
            tags: options.Tags));
    }
}