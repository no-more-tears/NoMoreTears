namespace NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;

/// <summary>
///     Data required to change the password of a signed-in user.
/// </summary>
/// <remarks>
///     All sessions of the user are revoked after the change, except the one
///     identified by <paramref name="CurrentSessionId"/> when it is provided.
/// </remarks>
/// <param name="UserId">Identifier of the user.</param>
/// <param name="CurrentPassword">Current plain-text password.</param>
/// <param name="NewPassword">New plain-text password.</param>
/// <param name="CurrentSessionId">Session that should survive the change.</param>
public sealed record ChangePasswordRequest(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    Guid? CurrentSessionId = null);
