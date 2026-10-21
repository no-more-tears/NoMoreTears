using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Services;

/// <summary>
///     Administrative operations over user accounts.
/// </summary>
/// <remarks>
///     Authorization of the caller is the responsibility of the host.
/// </remarks>
public interface IUserAdministrationService
{
    /// <summary>
    ///     Replaces the roles of the user.
    /// </summary>
    /// <remarks>
    ///     Roles are embedded into access tokens, so the change takes effect on the next
    ///     token refresh at the latest.
    /// </remarks>
    Task UpdateRolesAsync(
        Guid userId,
        IReadOnlyCollection<UserRole> roles,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Blocks the account from signing in and revokes every session.
    /// </summary>
    Task DeactivateAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Allows a deactivated account to sign in again and clears a pending lockout.
    /// </summary>
    Task ReactivateAsync(
        Guid userId,
        CancellationToken cancellationToken);
}