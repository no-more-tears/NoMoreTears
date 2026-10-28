using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using NoMoreTears.Shared.Providers.Identity.Contracts.Users;

namespace NoMoreTears.Modules.Identity.Applications.Services.Mappings;

/// <summary>
///     Maps identity provider results to module responses.
/// </summary>
internal static class ResponseMappings
{
    public static TokenResponse ToResponse(this TokenPair tokens) =>
        new(
            AccessToken: tokens.AccessToken,
            AccessTokenExpiresAt: tokens.AccessTokenExpiresAt,
            RefreshToken: tokens.RefreshToken,
            RefreshTokenExpiresAt: tokens.RefreshTokenExpiresAt);

    /// <remarks>
    ///     The challenge code is delivered out of band and is never mapped.
    /// </remarks>
    public static MfaChallengeResponse ToResponse(this MfaChallenge challenge) =>
        new(
            ChallengeToken: challenge.ChallengeToken,
            ExpiresAt: challenge.ExpiresAt);

    public static LoginResponse ToResponse(this LoginResult result) =>
        result.IsMfaRequired
            ? new LoginResponse(IsMfaRequired: true, Tokens: null, MfaChallenge: result.MfaChallenge.ToResponse())
            : new LoginResponse(IsMfaRequired: false, Tokens: result.Tokens.ToResponse(), MfaChallenge: null);

    public static UserResponse ToResponse(this UserInfo user) =>
        new(
            Id: user.Id,
            Email: user.Email,
            Username: user.Username,
            EmailConfirmed: user.EmailConfirmed,
            IsActive: user.IsActive,
            MfaEnabled: user.MfaEnabled,
            Roles: user.Roles,
            CreatedAt: user.CreatedAt,
            LastLoginAt: user.LastLoginAt);

    public static SessionResponse ToResponse(this SessionInfo session) =>
        new(
            SessionId: session.SessionId,
            CreatedAt: session.CreatedAt,
            LastActivityAt: session.LastActivityAt,
            ExpiresAt: session.ExpiresAt,
            MfaVerified: session.MfaVerified,
            IpAddress: session.IpAddress,
            UserAgent: session.UserAgent,
            DeviceId: session.DeviceId,
            IsCurrent: session.IsCurrent);
}