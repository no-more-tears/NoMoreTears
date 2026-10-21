namespace NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;

/// <summary>
///     Explains why a session was revoked.
/// </summary>
public enum SessionRevocationReason
{
    /// <summary>
    ///     The user signed out of the session.
    /// </summary>
    Logout,

    /// <summary>
    ///     The user signed out of all sessions.
    /// </summary>
    LogoutAll,

    /// <summary>
    ///     The user revoked the session from the session list.
    /// </summary>
    RevokedByUser,

    /// <summary>
    ///     The password was changed.
    /// </summary>
    PasswordChanged,

    /// <summary>
    ///     The password was reset through a recovery token.
    /// </summary>
    PasswordReset,

    /// <summary>
    ///     A previously rotated refresh token was presented again.
    /// </summary>
    ReuseDetected,

    /// <summary>
    ///     The account was deactivated.
    /// </summary>
    AccountDeactivated
}
