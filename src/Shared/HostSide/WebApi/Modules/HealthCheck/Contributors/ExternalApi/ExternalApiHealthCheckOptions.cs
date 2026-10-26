using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.ExternalApi;

/// <summary>
///     Configuration options for the external HTTP health check probe.
/// </summary>
public sealed class ExternalApiHealthCheckOptions
{
    /// <summary>
    ///     Display name of the probe shown in health check responses.
    /// </summary>
    public string Name { get; init; } = "external-api";

    /// <summary>
    ///     URL of the external service to probe.
    /// </summary>
    public Uri? Url { get; init; }

    /// <summary>
    ///     HTTP status codes considered healthy.
    ///     Defaults to <see cref="HttpStatusCode.OK"/> only.
    /// </summary>
    public HttpStatusCode[] HealthyStatusCodes { get; init; } = [HttpStatusCode.OK];

    /// <summary>
    ///     Maximum time to wait for a response before considering the probe failed.
    ///     Defaults to 5 seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Maximum acceptable response time in milliseconds.
    ///     If exceeded, the probe reports <see cref="HealthStatus.Degraded"/> regardless of status code.
    ///     Set to <see langword="null"/> to disable latency threshold checking.
    ///     Defaults to 1000 ms.
    /// </summary>
    public int? DegradedThresholdMs { get; init; } = 1000;

    /// <summary>
    ///     Health status reported when the probe fails.
    ///     Defaults to <see cref="HealthStatus.Unhealthy"/>.
    /// </summary>
    public HealthStatus FailureStatus { get; init; } = HealthStatus.Unhealthy;

    /// <summary>
    ///     Tags associated with this probe.
    /// </summary>
    public string[] Tags { get; init; } = [HealthCheckTag.Readiness];
}