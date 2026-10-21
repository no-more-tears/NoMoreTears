using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;

/// <summary>
///     Outcome of a sign-in attempt with valid credentials.
/// </summary>
/// <remarks>
///     Exactly one of <see cref="Tokens"/> and <see cref="MfaChallenge"/> is set.
///     When <see cref="IsMfaRequired"/> is <see langword="true"/> the host delivers the
///     challenge code to the user and completes the sign-in through MFA verification.
/// </remarks>
public sealed record LoginResult
{
    private LoginResult(
        TokenPair? tokens,
        MfaChallenge? mfaChallenge)
    {
        Tokens = tokens;
        MfaChallenge = mfaChallenge;
    }

    /// <summary>
    ///     Tokens issued when the sign-in completed without a second factor.
    /// </summary>
    public TokenPair? Tokens { get; }

    /// <summary>
    ///     Pending challenge when the account requires a second factor.
    /// </summary>
    public MfaChallenge? MfaChallenge { get; }

    /// <summary>
    ///     Indicates whether the sign-in has to be completed with MFA verification.
    /// </summary>
    [MemberNotNullWhen(true, nameof(MfaChallenge))]
    [MemberNotNullWhen(false, nameof(Tokens))]
    public bool IsMfaRequired => MfaChallenge is not null;

    /// <summary>
    ///     Creates a result for a sign-in that completed with issued tokens.
    /// </summary>
    public static LoginResult Authenticated(TokenPair tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        return new LoginResult(tokens, null);
    }

    /// <summary>
    ///     Creates a result for a sign-in that is pending MFA verification.
    /// </summary>
    public static LoginResult MfaRequired(MfaChallenge challenge)
    {
        ArgumentNullException.ThrowIfNull(challenge);
        return new LoginResult(null, challenge);
    }
}
