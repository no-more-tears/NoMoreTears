using System.Globalization;

namespace NoMoreTears.Shared.Providers.Identity.Diagnostics;

/// <summary>
///     Messages of exceptions thrown by the identity provider.
/// </summary>
internal static class IdentityMessages
{
    public const string InvalidCredentials = "Invalid credentials.";

    public const string InvalidToken = "The token is invalid or has expired.";

    public const string InvalidRefreshToken = "The refresh token is invalid or has expired.";

    public const string InvalidMfaCode = "The verification code is invalid or has expired.";

    public const string AccountLocked = "The account is temporarily locked after too many failed sign-in attempts.";

    public const string AccountDeactivated = "The account has been deactivated.";

    public const string EmailNotConfirmed = "The email address has not been confirmed.";

    public const string MfaRequiresConfirmedEmail = "Multi-factor authentication requires a confirmed email address.";

    public const string DuplicateUser = "A user with this email or username already exists.";

    public const string UserNotFound = "User not found.";

    public static string PasswordTooShort(int minimumLength) =>
        string.Create(CultureInfo.InvariantCulture, $"The password must be at least {minimumLength} characters long.");

    public static string PasswordTooLong(int maximumLength) =>
        string.Create(CultureInfo.InvariantCulture, $"The password must be at most {maximumLength} characters long.");
}