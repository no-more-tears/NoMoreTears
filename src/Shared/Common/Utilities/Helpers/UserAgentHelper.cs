using Microsoft.AspNetCore.Http;

namespace NoMoreTears.Shared.Common.Utilities.Helpers;

/// <summary>
///     Helper class for retrieving the client's User-Agent from an HTTP request.
/// </summary>
public static class UserAgentHelper
{
    /// <summary>Header name for the User-Agent.</summary>
    private const string UserAgentHeader = "User-Agent";

    /// <summary>
    ///     Retrieves the client's User-Agent string from the HTTP request.
    /// </summary>
    public static string? GetUserAgent(this HttpRequest httpRequest)
    {
        return TryGetUserAgentFromHeader(httpRequest, UserAgentHeader);
    }

    /// <summary>
    ///     Attempts to extract the User-Agent from a specific request header.
    /// </summary>
    private static string? TryGetUserAgentFromHeader(
        HttpRequest httpRequest,
        string headerName)
    {
        if (httpRequest.Headers.TryGetValue(headerName, out var value))
        {
            return value.ToString();
        }
        return null;
    }
}