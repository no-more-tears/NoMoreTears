using NoMoreTears.Shared.Providers.Identity.Contracts.Common;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Tokens;

/// <summary>
///     Creates and verifies one-time multi-factor authentication challenges.
/// </summary>
/// <remarks>
///     Challenges are bound to the client that started them, expire after a short period,
///     allow a limited number of attempts and can be verified once.
/// </remarks>
public interface IMfaChallengeService
{
    /// <summary>
    ///     Creates a challenge for the user, invalidating outstanding challenges.
    /// </summary>
    Task<MfaChallenge> CreateAsync(
        Guid userId,
        string email,
        ClientContext client,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Verifies the code and consumes the challenge on success.
    /// </summary>
    /// <returns>Identifier of the user the challenge belongs to, or <see langword="null"/> when verification failed.</returns>
    Task<Guid?> VerifyAsync(
        string challengeToken,
        string code,
        ClientContext client,
        CancellationToken cancellationToken);
}