namespace NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

/// <summary>
///     A one-time multi-factor authentication challenge.
/// </summary>
internal sealed class MfaChallengeEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public required string ChallengeHash { get; set; }

    /// <summary>
    ///     Hash of the code bound to the challenge token, so a leaked table does not expose codes.
    /// </summary>
    public required string CodeHash { get; set; }

    /// <summary>
    ///     Hash of the client that started the challenge; verification must come from the same client.
    /// </summary>
    public required string ClientFingerprint { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? ConsumedAt { get; set; }

    public int FailedAttempts { get; set; }
}