using Microsoft.EntityFrameworkCore;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity.Persistence;

/// <summary>
///     EF Core context owning the identity schema.
/// </summary>
public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    /// <summary>
    ///     Database schema that holds every identity table.
    /// </summary>
    public const string Schema = "identity";

    /// <summary>
    ///     Registered users.
    /// </summary>
    internal DbSet<UserEntity> Users => Set<UserEntity>();

    /// <summary>
    ///     Roles assigned to users.
    /// </summary>
    internal DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();

    /// <summary>
    ///     Sessions started by sign-ins.
    /// </summary>
    internal DbSet<SessionEntity> Sessions => Set<SessionEntity>();

    /// <summary>
    ///     Hashed refresh tokens of sessions.
    /// </summary>
    internal DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    /// <summary>
    ///     One-time multi-factor authentication challenges.
    /// </summary>
    internal DbSet<MfaChallengeEntity> MfaChallenges => Set<MfaChallengeEntity>();

    /// <summary>
    ///     Single-use tokens for email confirmation and password reset.
    /// </summary>
    internal DbSet<UserActionTokenEntity> UserActionTokens => Set<UserActionTokenEntity>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}