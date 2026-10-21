namespace NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

/// <summary>
///     A registered user with credentials and account state.
/// </summary>
internal sealed class UserEntity
{
    public Guid Id { get; set; }

    public required string Email { get; set; }

    /// <summary>
    ///     Upper-case invariant form of <see cref="Email"/> used for unique lookups.
    /// </summary>
    public required string NormalizedEmail { get; set; }

    public required string Username { get; set; }

    /// <summary>
    ///     Upper-case invariant form of <see cref="Username"/> used for unique lookups.
    /// </summary>
    public required string NormalizedUsername { get; set; }

    public required string PasswordHash { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool IsActive { get; set; } = true;

    public bool MfaEnabled { get; set; }

    /// <summary>
    ///     Consecutive failed sign-in attempts since the last successful one.
    /// </summary>
    public int FailedAccessAttempts { get; set; }

    /// <summary>
    ///     Moment until which sign-in is rejected because of too many failed attempts.
    /// </summary>
    public DateTimeOffset? LockoutEndAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? PasswordChangedAt { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public List<UserRoleEntity> Roles { get; } = [];
}