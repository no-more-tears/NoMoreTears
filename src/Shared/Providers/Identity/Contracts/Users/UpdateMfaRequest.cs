namespace NoMoreTears.Shared.Providers.Identity.Contracts.Users;

/// <summary>
///     Data required to enable or disable multi-factor authentication for a user.
/// </summary>
/// <param name="UserId">Identifier of the user.</param>
/// <param name="Password">Current plain-text password, required to re-authenticate the user.</param>
/// <param name="Enabled">Desired state of multi-factor authentication.</param>
public sealed record UpdateMfaRequest(
    Guid UserId,
    string Password,
    bool Enabled);
