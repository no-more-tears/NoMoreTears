using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Redis;

/// <summary>
///     Executes a PING command against Redis and reports degraded status. 
/// </summary>
internal sealed class RedisHealthCheck(
    RedisHealthCheckOptions options) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var configurationOptions = ConfigurationOptions.Parse(options.ConnectionString);

            if (!string.IsNullOrEmpty(options.Password))
            {
                configurationOptions.Password = options.Password;
            }

            using var connection = await ConnectionMultiplexer.ConnectAsync(configurationOptions);

            var db = connection.GetDatabase();
            var elapsed = await db.PingAsync();

            if (options.DegradedThresholdMs.HasValue
                && elapsed.TotalMilliseconds > options.DegradedThresholdMs.Value)
            {
                return HealthCheckResult.Degraded(
                    $"Redis responded in {elapsed.TotalMilliseconds:F2}ms, exceeding the threshold of {options.DegradedThresholdMs}ms.");
            }

            return HealthCheckResult.Healthy($"Redis is reachable. Round-trip time: {elapsed.TotalMilliseconds:F2}ms.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unreachable.", ex);
        }
    }
}