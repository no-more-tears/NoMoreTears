using Microsoft.AspNetCore.Http;

namespace NoMoreTears.Shared.HostSide.WebApi.Helpers;

/// <summary>
///     Helper class for managing cookies in the application.
/// </summary>
public static class CookieHelper
{
    /// <summary>Name of the cookie used to store the access token.</summary>
    public static readonly string AccessTokenCookieName = "access-token";

    /// <summary>Name of the cookie used to store the refresh token.</summary>
    public static readonly string RefreshTokenCookieName = "refresh-token";

    /// <summary>
    ///     Sets the access token cookie with specified options.
    /// </summary>
    public static void SetAccessTokenCookie(
        string accessToken,
        IResponseCookies responseCookies)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5),
        };

        responseCookies.Append(AccessTokenCookieName, accessToken, cookieOptions);
    }

    /// <summary>
    ///     Sets the refresh token cookie with specified options.
    /// </summary>
    public static void SetRefreshTokenCookie(
        string refreshToken,
        IResponseCookies responseCookies)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
        };

        responseCookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    /// <summary>
    ///     Clears the specified cookie by setting its expiration date to the past.
    /// </summary>
    public static void ClearCookies(
        string name, 
        IResponseCookies responseCookies)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
        };

        responseCookies.Delete(name, cookieOptions);
    }
}