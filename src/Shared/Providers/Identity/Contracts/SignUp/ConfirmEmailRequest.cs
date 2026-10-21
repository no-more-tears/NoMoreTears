namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignUp;

/// <summary>
///     Data required to confirm an email address.
/// </summary>
/// <param name="UserId">Identifier of the user who owns the token.</param>
/// <param name="Token">Single-use confirmation token that was delivered to the user.</param>
public sealed record ConfirmEmailRequest(
    Guid UserId,
    string Token);
