namespace NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;

/// <summary>
///     Data required to reset a forgotten password.
/// </summary>
/// <param name="UserId">Identifier of the user who owns the token.</param>
/// <param name="Token">Single-use reset token that was delivered to the user.</param>
/// <param name="NewPassword">New plain-text password.</param>
public sealed record ResetPasswordRequest(
    Guid UserId,
    string Token,
    string NewPassword);
