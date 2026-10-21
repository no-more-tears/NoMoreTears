using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Users;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Services;

/// <summary>
///     Self-service operations of a user over its own account.
/// </summary>
public interface IAccountService
{
    /// <summary>
    ///     Returns identity data of the user or <see langword="null"/> when the user does not exist.
    /// </summary>
    Task<UserInfo?> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Re-authenticates the user with the password before a sensitive operation.
    /// </summary>
    Task VerifyPasswordAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Changes the password after verifying the current one and revokes other sessions.
    /// </summary>
    Task ChangePasswordAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Issues a password reset ticket, invalidating previously issued ones.
    /// </summary>
    /// <remarks>
    ///     Returns <see langword="null"/> when no active user matches the identifier,
    ///     so callers cannot probe for existing accounts.
    /// </remarks>
    Task<UserActionTicket?> ForgotPasswordAsync(
        string identifier,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Sets a new password with a single-use reset token and revokes every session.
    /// </summary>
    /// <remarks>
    ///     A successful reset also confirms the email address and clears a pending lockout.
    /// </remarks>
    Task ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Enables or disables multi-factor authentication after re-authenticating the user.
    /// </summary>
    Task UpdateMfaAsync(
        UpdateMfaRequest request,
        CancellationToken cancellationToken);
}