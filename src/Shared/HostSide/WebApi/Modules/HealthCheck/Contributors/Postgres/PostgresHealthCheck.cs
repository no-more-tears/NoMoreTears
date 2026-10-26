using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Postgres;

/// <summary>
///     Executes a lightweight SQL query against PostgreSQL and 
///     reports degraded status. 
/// </summary>
internal sealed class PostgresHealthCheck(
    PostgresHealthCheckOptions options) : IHealthCheck
{
    private const string HealthQuery = "SELECT 1;";

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken)
    {
        var started = DateTimeOffset.UtcNow;

        try
        {
            await using var connection = new NpgsqlConnection(options.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new NpgsqlCommand(HealthQuery, connection)
            {
                CommandTimeout = options.CommandTimeout
            };

            _ = await command.ExecuteScalarAsync(cancellationToken);

            var elapsed = (DateTimeOffset.UtcNow - started).TotalMilliseconds;

            if (options.DegradedThresholdMs.HasValue && elapsed > options.DegradedThresholdMs.Value)
            {
                return HealthCheckResult.Degraded(
                    $"PostgreSQL responded in {elapsed:F2}ms, exceeding the threshold of {options.DegradedThresholdMs}ms.");
            }

            return HealthCheckResult.Healthy($"PostgreSQL is reachable. Response time: {elapsed:F2}ms.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unreachable.", ex);
        }
    }
}