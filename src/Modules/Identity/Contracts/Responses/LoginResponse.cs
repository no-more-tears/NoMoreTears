namespace NoMoreTears.Modules.Identity.Contracts.Responses;

/// <remarks>
///     Exactly one of <paramref name="Tokens"/> and <paramref name="MfaChallenge"/> is set.
/// </remarks>
public sealed record LoginResponse(
    bool IsMfaRequired,
    TokenResponse? Tokens,
    MfaChallengeResponse? MfaChallenge);