using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Services;

/// <summary>
///     Authenticates users and manages the lifecycle of their tokens.
/// </summary>
public interface ISignInService
{
    /// <summary>
    ///     Verifies the credentials and either starts a session or issues an MFA challenge.
    /// </summary>
    /// <remarks>
    ///     Failed attempts are counted towards the account lockout policy.
    /// </remarks>
    Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Completes a sign-in that is pending MFA verification and starts a session.
    /// </summary>
    Task<TokenPair> VerifyMfaAsync(
        VerifyMfaRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Rotates the refresh token and issues a new token pair for the same session.
    /// </summary>
    /// <remarks>
    ///     Presenting an already rotated token is treated as a replay and revokes the whole session.
    /// </remarks>
    Task<TokenPair> RefreshAsync(
        RefreshRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Revokes the session the refresh token belongs to. Unknown tokens are ignored.
    /// </summary>
    Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Revokes every active session of the user.
    /// </summary>
    Task LogoutAllAsync(
        Guid userId,
        CancellationToken cancellationToken);
}