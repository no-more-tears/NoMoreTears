using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity;

/// <summary>
///     User administration capability of <see cref="IdentityProvider"/>.
/// </summary>
internal sealed partial class IdentityProvider
{
    /// <inheritdoc />
    public async Task UpdateRolesAsync(
        Guid userId,
        IReadOnlyCollection<UserRole> roles,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roles);

        var user = await RequireUserAsync(userId, cancellationToken);
        var desired = roles.ToHashSet();
        var existing = user.Roles.Select(x => x.Role).ToHashSet();

        user.Roles.RemoveAll(x => !desired.Contains(x.Role));

        foreach (var role in desired.Except(existing))
        {
            user.Roles.Add(new UserRoleEntity { UserId = user.Id, Role = role });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeactivateAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await RequireUserAsync(userId, cancellationToken);

        if (!user.IsActive)
        {
            return;
        }

        user.IsActive = false;
        await db.SaveChangesAsync(cancellationToken);

        await RevokeSessionsAsync(
            user.Id,
            SessionRevocationReason.AccountDeactivated,
            timeProvider.GetUtcNow(),
            exceptSessionId: null,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task ReactivateAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await RequireUserAsync(userId, cancellationToken);

        user.IsActive = true;
        user.FailedAccessAttempts = 0;
        user.LockoutEndAt = null;

        await db.SaveChangesAsync(cancellationToken);
    }
}