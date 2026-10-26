using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace NoMoreTears.Shared.HostSide.RateLimit;

/// <summary>
///     Provides methods to generate rate limit keys.
/// </summary>
public static class RateLimitKeyProvider
{
    /// <summary>
    ///     JWT subject claim; inbound claim type mapping is disabled, so the short name is used as-is.
    /// </summary>
    private const string SubjectClaimType = "sub";

    /// <summary>
    ///     Generates a key shared by all endpoints, so the client has a single budget for the whole API.
    /// </summary>
    public static string ForClient(HttpContext httpContext) =>
        GetClientId(httpContext);

    /// <summary>
    ///     Generates a key for the current endpoint, so the client has a separate budget for each route.
    /// </summary>
    public static string ForEndpoint(HttpContext httpContext) =>
        FromParts(GetClientId(httpContext), httpContext.Request.Method, GetRoute(httpContext));

    /// <summary>
    ///     Generates a rate limit key based on provided parts.
    /// </summary>
    public static string FromParts(string identifier, string method, string path) =>
        $"{identifier}:{method.ToUpperInvariant()}:{path.ToLowerInvariant()}";

    /// <summary>
    ///     Authenticated users are limited by their id, so users behind the same NAT do not share a budget.
    ///     Anonymous clients fall back to the IP address.
    /// </summary>
    private static string GetClientId(HttpContext httpContext)
    {
        var userId = httpContext.User.FindFirst(SubjectClaimType)?.Value;

        return string.IsNullOrEmpty(userId)
            ? $"ip:{GetClientIp(httpContext)}"
            : $"user:{userId}";
    }

    /// <summary>
    ///     Uses the connection address only. Headers such as X-Forwarded-For are controlled by the client,
    ///     so they are trusted only after the forwarded headers middleware validated the proxy.
    /// </summary>
    private static string GetClientIp(HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress;

        if (ipAddress is { IsIPv4MappedToIPv6: true })
        {
            ipAddress = ipAddress.MapToIPv4();
        }

        return ipAddress?.ToString() ?? "unknown";
    }

    /// <summary>
    ///     Prefers the route template so requests to /users/1 and /users/2 share the same limit.
    /// </summary>
    private static string GetRoute(HttpContext httpContext) =>
        (httpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText
        ?? httpContext.Request.Path.Value
        ?? "/";
}