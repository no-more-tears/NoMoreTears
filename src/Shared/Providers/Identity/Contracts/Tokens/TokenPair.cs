namespace NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;

/// <summary>
///     Access and refresh tokens issued for a session.
/// </summary>
/// <param name="AccessToken">Signed JWT access token.</param>
/// <param name="AccessTokenExpiresAt">Moment after which the access token is rejected.</param>
/// <param name="RefreshToken">Opaque single-use refresh token.</param>
/// <param name="RefreshTokenExpiresAt">Moment after which the refresh token is rejected.</param>
/// <param name="SessionId">Identifier of the session the tokens belong to.</param>
public sealed record TokenPair(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    Guid SessionId);
