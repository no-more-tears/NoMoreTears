using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.HostSide.RateLimit;
using NoMoreTears.Shared.HostSide.RateLimit.Abstractions;
using NoMoreTears.Shared.HostSide.RateLimit.Configuration;
using NoMoreTears.Shared.HostSide.RateLimit.Models;
using StackExchange.Redis;

namespace NoMoreTears.Shared.HostSide.Endpoints.Extensions;

/// <summary>
///     Extensions for endpoint configuration.
/// </summary>
public static class EndpointExtensions
{
    private const string TooManyRequestsType = "https://datatracker.ietf.org/doc/html/rfc6585#section-4";

    /// <summary>
    ///     Applies the global <see cref="RateLimitOptions"/> rate limiter to an endpoint or a route group.
    ///     The client has a single budget shared by all endpoints it is applied to.
    /// </summary>
    public static TBuilder WithRateLimit<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.AddEndpointFilterFactory((factoryContext, next) =>
        {
            var services = factoryContext.ApplicationServices;

            return CreateRateLimitFilter(
                services.GetRequiredService<IRateLimiter>(),
                services.GetRequiredService<IOptions<RateLimitOptions>>().Value,
                services.GetRequiredService<TimeProvider>(),
                RateLimitKeyProvider.ForClient,
                next);
        });

    /// <summary>
    ///     Applies a per-endpoint rate limiter on top of the global one.
    ///     The client has a separate budget for each endpoint.
    /// </summary>
    public static TBuilder WithRateLimit<TBuilder>(
        this TBuilder builder,
        int maxRequests,
        int windowSeconds = 60)
        where TBuilder : IEndpointConventionBuilder
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRequests);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowSeconds);

        return builder.AddEndpointFilterFactory((factoryContext, next) =>
        {
            var services = factoryContext.ApplicationServices;
            var timeProvider = services.GetRequiredService<TimeProvider>();
            var globalOptions = services.GetRequiredService<IOptions<RateLimitOptions>>().Value;

            var options = new RateLimitOptions
            {
                MaxRequests = maxRequests,
                WindowSizeInSeconds = windowSeconds,
                StatusCode = globalOptions.StatusCode,
                AddRateLimitHeaders = globalOptions.AddRateLimitHeaders,
                FailOpen = globalOptions.FailOpen
            };

            var limiter = new SlidingWindowRateLimiter(
                services.GetRequiredService<IConnectionMultiplexer>(),
                timeProvider,
                Options.Create(options),
                services.GetRequiredService<ILogger<SlidingWindowRateLimiter>>());

            return CreateRateLimitFilter(limiter, options, timeProvider, RateLimitKeyProvider.ForEndpoint, next);
        });
    }

    /// <summary>
    ///     Applies the global <see cref="ThrottleOptions"/> throttler to an endpoint or a route group.
    ///     The client has a single bucket shared by all endpoints it is applied to.
    /// </summary>
    public static TBuilder WithThrottle<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.AddEndpointFilterFactory((factoryContext, next) =>
        {
            var services = factoryContext.ApplicationServices;

            return CreateThrottleFilter(
                services.GetRequiredService<IThrottler>(),
                services.GetRequiredService<IOptions<ThrottleOptions>>().Value,
                RateLimitKeyProvider.ForClient,
                next);
        });

    /// <summary>
    ///     Applies a per-endpoint throttler on top of the global one.
    ///     The client has a separate bucket for each endpoint.
    /// </summary>
    public static TBuilder WithThrottle<TBuilder>(
        this TBuilder builder,
        int requestsPerSecond,
        double burstSeconds = 1.0)
        where TBuilder : IEndpointConventionBuilder
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestsPerSecond);
        ArgumentOutOfRangeException.ThrowIfLessThan(requestsPerSecond * burstSeconds, 1.0, nameof(burstSeconds));

        return builder.AddEndpointFilterFactory((factoryContext, next) =>
        {
            var services = factoryContext.ApplicationServices;
            var globalOptions = services.GetRequiredService<IOptions<ThrottleOptions>>().Value;

            var options = new ThrottleOptions
            {
                RequestsPerSecond = requestsPerSecond,
                BurstSeconds = burstSeconds,
                StatusCode = globalOptions.StatusCode,
                AddRetryAfterHeader = globalOptions.AddRetryAfterHeader,
                FailOpen = globalOptions.FailOpen
            };

            var throttler = new TokenBucketThrottler(
                services.GetRequiredService<IConnectionMultiplexer>(),
                services.GetRequiredService<TimeProvider>(),
                Options.Create(options),
                services.GetRequiredService<ILogger<TokenBucketThrottler>>());

            return CreateThrottleFilter(throttler, options, RateLimitKeyProvider.ForEndpoint, next);
        });
    }

    /// <summary>
    ///     The rejection is returned as a result instead of an exception,
    ///     because the exception handler clears the response headers.
    /// </summary>
    private static EndpointFilterDelegate CreateRateLimitFilter(
        IRateLimiter limiter,
        RateLimitOptions options,
        TimeProvider timeProvider,
        Func<HttpContext, string> keySelector,
        EndpointFilterDelegate next) =>
        async context =>
        {
            var httpContext = context.HttpContext;

            var key = keySelector(httpContext);
            var decision = await limiter.CheckAsync(key, httpContext.RequestAborted);

            if (options.AddRateLimitHeaders)
            {
                ApplyRateLimitHeaders(httpContext, decision);
            }

            if (decision.IsAllowed)
            {
                return await next(context);
            }

            var retryAfterSeconds = GetRetryAfterSeconds(decision, timeProvider);

            if (options.AddRateLimitHeaders)
            {
                ApplyRetryAfterHeader(httpContext, retryAfterSeconds);
            }

            return BuildRejection(options.StatusCode, BuildRateLimitMessage(retryAfterSeconds));
        };

    private static EndpointFilterDelegate CreateThrottleFilter(
        IThrottler throttler,
        ThrottleOptions options,
        Func<HttpContext, string> keySelector,
        EndpointFilterDelegate next) =>
        async context =>
        {
            var httpContext = context.HttpContext;

            var key = keySelector(httpContext);
            var waitSeconds = await throttler.TryConsumeAsync(key, httpContext.RequestAborted);

            if (!waitSeconds.HasValue)
            {
                return await next(context);
            }

            if (options.AddRetryAfterHeader)
            {
                ApplyRetryAfterHeader(httpContext, waitSeconds.Value);
            }

            return BuildRejection(options.StatusCode, BuildThrottleMessage(waitSeconds.Value));
        };

    private static void ApplyRateLimitHeaders(
        HttpContext httpContext,
        RateLimitDecision decision)
    {
        httpContext.Response.Headers["X-RateLimit-Limit"] = decision.MaxRequests.ToString();
        httpContext.Response.Headers["X-RateLimit-Remaining"] = decision.Remaining.ToString();
        httpContext.Response.Headers["X-RateLimit-Reset"] = decision.ResetsAt.ToUnixTimeSeconds().ToString();
    }

    private static void ApplyRetryAfterHeader(
        HttpContext httpContext,
        double waitSeconds) =>
        httpContext.Response.Headers["Retry-After"] = Math.Ceiling(waitSeconds).ToString();

    private static int GetRetryAfterSeconds(
        RateLimitDecision decision,
        TimeProvider timeProvider) =>
        Math.Max(1, (int)Math.Ceiling((decision.ResetsAt - timeProvider.GetUtcNow()).TotalSeconds));

    private static IResult BuildRejection(int statusCode, string detail) =>
        Results.Problem(
            detail: detail,
            statusCode: statusCode,
            title: ReasonPhrases.GetReasonPhrase(statusCode),
            type: statusCode == StatusCodes.Status429TooManyRequests ? TooManyRequestsType : null);

    private static string BuildRateLimitMessage(double retryAfterSeconds) =>
        $"You have exceeded the allowed number of requests. Please try again in {retryAfterSeconds} seconds.";

    private static string BuildThrottleMessage(double retryAfterSeconds) =>
        $"You are sending requests too fast. Please try again in {retryAfterSeconds} seconds.";
}