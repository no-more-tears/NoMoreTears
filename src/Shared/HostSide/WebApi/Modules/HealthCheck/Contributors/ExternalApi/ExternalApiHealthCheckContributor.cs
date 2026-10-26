using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.ExternalApi;

/// <summary>
///     Registers an HTTP GET health check probe against an external service URL.
/// </summary>
public sealed class ExternalApiHealthCheckContributor(
    ExternalApiHealthCheckOptions options) : IHealthCheckContributor
{
    /// <inheritdoc />
    public void Register(IHealthChecksBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Add(new HealthCheckRegistration(
            name: options.Name,
            factory: sp => new ExternalApiHealthCheck(options, sp.GetRequiredService<IHttpClientFactory>()),
            failureStatus: options.FailureStatus,
            tags: options.Tags));
    }
}