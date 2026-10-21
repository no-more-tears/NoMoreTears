namespace NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;

/// <summary>
///     An active session of a user.
/// </summary>
/// <param name="SessionId">Identifier of the session.</param>
/// <param name="CreatedAt">Moment the session was started.</param>
/// <param name="LastActivityAt">Moment of the most recent token refresh.</param>
/// <param name="ExpiresAt">Moment after which the session can no longer be refreshed.</param>
/// <param name="MfaVerified">Indicates whether the session was verified with a second factor.</param>
/// <param name="IpAddress">IP address of the client at the most recent activity.</param>
/// <param name="UserAgent">User agent of the client that started the session.</param>
/// <param name="DeviceId">Device identifier of the client that started the session.</param>
/// <param name="IsCurrent">Indicates whether this is the session of the caller.</param>
public sealed record SessionInfo(
    Guid SessionId,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastActivityAt,
    DateTimeOffset ExpiresAt,
    bool MfaVerified,
    string? IpAddress,
    string? UserAgent,
    string? DeviceId,
    bool IsCurrent);
