namespace NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;

/// <summary>
///     Values of the <c>amr</c> claim as defined by RFC 8176.
/// </summary>
public static class AuthenticationMethods
{
    /// <summary>
    ///     The session was authenticated with a password.
    /// </summary>
    public const string Password = "pwd";

    /// <summary>
    ///     The session was additionally verified with a one-time code.
    /// </summary>
    public const string OneTimeCode = "otp";
}
