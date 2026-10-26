using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.ExternalApi;

/// <summary>
///     Sends a GET request to an external service and reports the result
///     based on the response status code and latency threshold.
/// </summary>
internal sealed class ExternalApiHealthCheck(
    ExternalApiHealthCheckOptions options,
    IHttpClientFactory httpClientFactory) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var started = DateTimeOffset.UtcNow;

        try
        {
            using var client = httpClientFactory.CreateClient();
            client.Timeout = options.Timeout;

            using var response = await client.GetAsync(options.Url, cancellationToken);

            var elapsed = (DateTimeOffset.UtcNow - started).TotalMilliseconds;

            if (!options.HealthyStatusCodes.Contains(response.StatusCode))
            {
                return HealthCheckResult.Unhealthy(
                    $"{options.Url} returned unexpected status code {(int)response.StatusCode} {response.StatusCode}.");
            }
            if (options.DegradedThresholdMs.HasValue && elapsed > options.DegradedThresholdMs.Value)
            {
                return HealthCheckResult.Degraded(
                    $"{options.Url} responded in {elapsed:F2}ms, exceeding the threshold of {options.DegradedThresholdMs}ms.");
            }

            return HealthCheckResult.Healthy($"{options.Url} is reachable. Response time: {elapsed:F2}ms.");
        }
        catch (TaskCanceledException)
        {
            return HealthCheckResult.Unhealthy($"{options.Url} did not respond within {options.Timeout.TotalSeconds:F0}s.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy( $"{options.Url} is unreachable.", ex);
        }
    }
}