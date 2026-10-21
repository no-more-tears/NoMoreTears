namespace NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;

/// <summary>
///     A signed JWT access token together with its metadata.
/// </summary>
/// <param name="Value">Compact serialized JWT.</param>
/// <param name="TokenId">Value of the <c>jti</c> claim.</param>
/// <param name="ExpiresAt">Moment after which the token is rejected.</param>
public sealed record AccessToken(
    string Value,
    string TokenId,
    DateTimeOffset ExpiresAt);
