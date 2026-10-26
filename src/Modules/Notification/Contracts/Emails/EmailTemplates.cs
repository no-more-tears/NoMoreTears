namespace NoMoreTears.Modules.Notification.Contracts.Emails;

/// <summary>
///     Names of the email templates shipped with the notification module.
/// </summary>
public static class EmailTemplates
{
    /// <summary>
    ///     Asks the user to confirm the email address.
    ///     Model: <c>Email</c>, <c>Link</c>, <c>ExpiresAt</c>.
    /// </summary>
    public const string EmailConfirmation = nameof(EmailConfirmation);

    /// <summary>
    ///     Lets the user set a new password.
    ///     Model: <c>Email</c>, <c>Link</c>, <c>ExpiresAt</c>.
    /// </summary>
    public const string PasswordReset = nameof(PasswordReset);
}