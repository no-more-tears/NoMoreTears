namespace NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

/// <summary>
///     Purpose of a single-use user action token.
/// </summary>
public enum UserActionPurpose
{
    /// <summary>
    ///     Confirms ownership of the email address.
    /// </summary>
    EmailConfirmation,

    /// <summary>
    ///     Authorizes a password reset.
    /// </summary>
    PasswordReset
}
