using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Extensions;

/// <summary>
///     Extensions for reading identity data from <see cref="ICurrentUserSession"/>.
/// </summary>
public static class CurrentUserSessionExtensions
{
    /// <summary>
    ///     Describes the client of the current request.
    /// </summary>
    /// <remarks>
    ///     MFA challenges are bound to this context, so every sign-in step has to build it the same way.
    /// </remarks>
    public static ClientContext ToClientContext(this ICurrentUserSession currentUserSession)
    {
        ArgumentNullException.ThrowIfNull(currentUserSession);

        return new ClientContext(
            currentUserSession.IpAddress,
            currentUserSession.UserAgent,
            currentUserSession.DeviceName);
    }

    /// <summary>
    ///     Returns the identifier of the authenticated user.
    /// </summary>
    public static Guid GetRequiredUserId(this ICurrentUserSession currentUserSession)
    {
        ArgumentNullException.ThrowIfNull(currentUserSession);

        return currentUserSession.UserId ?? throw new UnauthorizedException();
    }
}
