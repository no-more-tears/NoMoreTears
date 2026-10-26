using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Configuration;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.ExternalApi;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Postgres;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck.Contributors.Redis;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck;

/// <summary>
///     Web module responsible for exposing a single aggregated health check endpoint
///     covering all registered infrastructure probes.
///     Open for extension via <see cref="IHealthCheckContributor"/>.
/// </summary>
public sealed class HealthCheckWebModule : IWebModule
{
    private const string Endpoint = "/health";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HealthCheckOptions _options;

    /// <summary>
    ///     Initializes the module with default options and no probes.
    /// </summary>
    public HealthCheckWebModule()
        : this(new HealthCheckOptions()) { }

    /// <summary>
    ///     Initializes the module with the specified health check options.
    /// </summary>
    public HealthCheckWebModule(HealthCheckOptions options)
    {
        _options = options;
    }

    /// <summary>
    ///     Initializes the module via a fluent configuration delegate.
    /// </summary>
    public HealthCheckWebModule(Action<HealthCheckOptions> configureOptions)
    {
        _options = new HealthCheckOptions();
        configureOptions(_options);
    }

    /// <inheritdoc />
    public int Order => WebModuleOrder.HealthChecks;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        var builder = context.Services.AddHealthChecks();

        if (_options.ApiEnabled)
        {
            // Adds API liveness probe. Requires calling the UseApi() method.
            new ExternalApiHealthCheckContributor(_options.ApiOptions!).Register(builder);
        }
        if (_options.PostgresEnabled)
        {
            // Adds PostgreSQL readiness probe. Requires calling the UsePostgres() method.
            new PostgresHealthCheckContributor(_options.PostgresOptions!).Register(builder);
        }
        if (_options.RedisEnabled)
        {
            // Adds Redis readiness probe. Requires calling the UseRedis() method.
            new RedisHealthCheckContributor(_options.RedisOptions!).Register(builder);
        }

        foreach (var contributor in _options.ExplicitContributors)
        {
            contributor.Register(builder);
        }
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
        app.UseHealthChecks(Endpoint, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = WriteResponseAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });
    }

    private async Task WriteResponseAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var response = new
        {
            status = report.Status.ToString(),
            duration = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            timestamp = DateTimeOffset.UtcNow,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString("c"),
                tags = entry.Value.Tags,
                error = entry.Value.Exception?.Message
            }),
            totalDuration = report.TotalDuration.ToString("c")
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}