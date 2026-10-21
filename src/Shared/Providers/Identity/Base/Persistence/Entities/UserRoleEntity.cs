using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

/// <summary>
///     A role assigned to a user.
/// </summary>
internal sealed class UserRoleEntity
{
    public Guid UserId { get; set; }

    public UserRole Role { get; set; }
}