using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;

/// <summary>
///     Data required to complete a sign-in that is pending MFA verification.
/// </summary>
/// <param name="ChallengeToken">Challenge token returned by the sign-in.</param>
/// <param name="Code">One-time verification code entered by the user.</param>
/// <param name="Client">Client that completes the sign-in; must match the client that started it.</param>
public sealed record VerifyMfaRequest(
    string ChallengeToken,
    string Code,
    ClientContext Client);
