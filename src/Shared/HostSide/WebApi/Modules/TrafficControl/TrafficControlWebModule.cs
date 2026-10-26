using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoMoreTears.Shared.HostSide.RateLimit;
using NoMoreTears.Shared.HostSide.RateLimit.Abstractions;
using NoMoreTears.Shared.HostSide.RateLimit.Configuration;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using System.Net;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.TrafficControl;

/// <summary>
///     Web module responsible for setting up request throttling and
///     rate limiting across the application.
/// </summary>
public sealed class TrafficControlWebModule : IWebModule
{
    /// <inheritdoc />
    public int Order => WebModuleOrder.TrafficControl;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        var trafficOptions = context.Configuration
            .GetSection(TrafficControlOptions.SectionName)
            .Get<TrafficControlOptions>()
            ?? new TrafficControlOptions();

        context.Services
            .TryAddSingleton(TimeProvider.System);

        context.Services
            .Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;

                foreach (var proxy in trafficOptions.KnownProxies)
                {
                    options.KnownProxies.Add(IPAddress.Parse(proxy));
                }
            });

        context.Services
            .AddOptions<RateLimitOptions>()
            .Bind(context.Configuration.GetSection(RateLimitOptions.SectionName))
            .Validate(
                options => options.MaxRequests > 0 && options.WindowSizeInSeconds > 0,
                $"{RateLimitOptions.SectionName}: MaxRequests and WindowSizeInSeconds must be positive.")
            .ValidateOnStart();

        context.Services
            .AddOptions<ThrottleOptions>()
            .Bind(context.Configuration.GetSection(ThrottleOptions.SectionName))
            .Validate(
                options => options.RequestsPerSecond > 0 && options.RequestsPerSecond * options.BurstSeconds >= 1.0,
                $"{ThrottleOptions.SectionName}: RequestsPerSecond must be positive and RequestsPerSecond * BurstSeconds must be at least 1.")
            .ValidateOnStart();

        context.Services
            .AddSingleton<IRateLimiter, SlidingWindowRateLimiter>()
            .AddSingleton<IThrottler, TokenBucketThrottler>();
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
        // Replaces the connection address with the X-Forwarded-For value only for trusted proxies.
        app.UseForwardedHeaders();
    }
}