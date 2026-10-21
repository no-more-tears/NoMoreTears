namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;

/// <summary>
///     A pending multi-factor authentication challenge.
/// </summary>
/// <remarks>
///     <see cref="ChallengeToken"/> is returned to the client and presented together with the code.
///     <see cref="Code"/> is delivered to the user out of band (for example by email)
///     and must never be returned to the client.
/// </remarks>
/// <param name="ChallengeToken">Opaque token identifying the challenge.</param>
/// <param name="Code">One-time verification code.</param>
/// <param name="ExpiresAt">Moment after which the challenge can no longer be verified.</param>
public sealed record MfaChallenge(
    string ChallengeToken,
    string Code,
    DateTimeOffset ExpiresAt);
