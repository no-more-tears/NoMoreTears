using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.HostSide.WebApi.Helpers;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Authorization;

/// <summary>
///     Creates <see cref="ICurrentUserSession"/> from the current HTTP request.
/// </summary>
public static class HttpUserSessionFactory
{
    private const string SessionIdClaim = "sid";
    private const string RoleClaim = "role";

    public static ICurrentUserSession Create(IServiceProvider provider)
    {
        var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;

        if (httpContext is null)
        {
            return new CurrentUserSession();
        }

        var user = httpContext.User;
        var request = httpContext.Request;

        if (user.Identity?.IsAuthenticated != true)
        {
            return new CurrentUserSession(
                userId: null,
                roles: null,
                sessionId: null,
                ipAddress: request.GetIpAddress()?.ToString(),
                userAgent: NullIfEmpty(request.GetUserAgent()));
        }

        return new CurrentUserSession(
            userId: ParseGuid(user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier)),
            roles: ParseRoles(user),
            sessionId: ParseGuid(user.FindFirstValue(SessionIdClaim)),
            ipAddress: request.GetIpAddress()?.ToString(),
            userAgent: NullIfEmpty(request.GetUserAgent()));
    }

    private static Guid? ParseGuid(string? value) =>
        Guid.TryParse(value, out var id) ? id : null;

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    private static List<UserRole> ParseRoles(ClaimsPrincipal user) =>
        user.FindAll(ClaimTypes.Role)
            .Concat(user.FindAll(RoleClaim))
            .Select(claim => Enum.TryParse<UserRole>(claim.Value, ignoreCase: true, out var role) ? role : (UserRole?)null)
            .OfType<UserRole>()
            .Distinct()
            .ToList();
}