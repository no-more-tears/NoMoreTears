using System.Net;
using Microsoft.AspNetCore.Http;

namespace NoMoreTears.Shared.Common.Utilities.Helpers;

/// <summary>
///     Helper class for retrieving the client's IP address from an HTTP request.
/// </summary>
public static class IpAddressHelper
{
    /// <summary>Header name for the real client IP (set by reverse proxies) </summary>
    private const string XRealIp = "X-Real-Ip";

    /// <summary>Header name for the original forwarded IP (used with multiple proxies).</summary>
    private const string XOriginalForwardedIp = "X-Original-Forwarded-Ip";

    /// <summary>
    ///     Retrieves the client's IP address from the HTTP request.
    /// </summary>
    public static IPAddress? GetIpAddress(this HttpRequest httpRequest)
    {
        return TryGetIpAddressFromHeader(httpRequest, XOriginalForwardedIp)
               ?? TryGetIpAddressFromHeader(httpRequest, XRealIp)
               ?? httpRequest.HttpContext.Connection.RemoteIpAddress;
    }

    /// <summary>
    ///     Attempts to extract an IP address from a specific request header.
    /// </summary>
    private static IPAddress? TryGetIpAddressFromHeader(
        HttpRequest httpRequest, 
        string headerName)
    {
        if (httpRequest.Headers.TryGetValue(headerName, out var value)
            && IPAddress.TryParse(value, out var ipAddress))
        {
            return ipAddress;
        }

        return null;
    }
}