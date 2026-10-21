using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;

namespace NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

/// <summary>
///     A session started by a sign-in; the lineage of its rotated refresh tokens.
/// </summary>
internal sealed class SessionEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Moment of the most recent refresh token rotation.
    /// </summary>
    public DateTimeOffset LastActivityAt { get; set; }

    /// <summary>
    ///     Absolute expiration; no rotation extends the session beyond it.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    public bool MfaVerified { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public SessionRevocationReason? RevocationReason { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceId { get; set; }
}