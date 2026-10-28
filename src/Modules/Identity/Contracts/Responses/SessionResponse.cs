namespace NoMoreTears.Modules.Identity.Contracts.Responses;

public sealed record SessionResponse(
    Guid SessionId,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastActivityAt,
    DateTimeOffset ExpiresAt,
    bool MfaVerified,
    string? IpAddress,
    string? UserAgent,
    string? DeviceId,
    bool IsCurrent);