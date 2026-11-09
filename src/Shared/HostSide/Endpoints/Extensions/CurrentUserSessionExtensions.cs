using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;

namespace NoMoreTears.Shared.HostSide.Endpoints.Extensions;

/// <summary>
///     Extensions for reading the current user in endpoint handlers.
/// </summary>
public static class CurrentUserSessionExtensions
{
    /// <summary>
    ///     Returns the identifier of the authenticated user.
    /// </summary>
    public static Guid GetRequiredUserId(this ICurrentUserSession currentUserSession)
    {
        ArgumentNullException.ThrowIfNull(currentUserSession);

        return currentUserSession.UserId ?? throw new UnauthorizedException();
    }

    /// <summary>
    ///     Returns whether the current user has the role.
    /// </summary>
    public static bool HasRole(this ICurrentUserSession currentUserSession, UserRole role)
    {
        ArgumentNullException.ThrowIfNull(currentUserSession);

        return currentUserSession.Roles?.Contains(role) ?? false;
    }
}
