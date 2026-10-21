namespace NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

/// <summary>
///     A hashed refresh token belonging to a session.
/// </summary>
internal sealed class RefreshTokenEntity
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    /// <summary>
    ///     Owning session; populated when loaded with an include or attached in the same unit of work.
    /// </summary>
    public SessionEntity Session { get; set; } = null!;

    public required string TokenHash { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    ///     Moment the token was rotated into its successor.
    /// </summary>
    public DateTimeOffset? ConsumedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }
}