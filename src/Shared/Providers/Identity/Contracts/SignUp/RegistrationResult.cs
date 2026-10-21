using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Shared.Providers.Identity.Contracts.SignUp;

/// <summary>
///     Outcome of a successful registration.
/// </summary>
/// <remarks>
///     The provider never sends email itself. The host delivers
///     <paramref name="EmailConfirmation"/> to the user through its own channel.
/// </remarks>
/// <param name="UserId">Identifier of the created user.</param>
/// <param name="EmailConfirmation">Ticket the user has to present to confirm the email address.</param>
/// <param name="RequiresEmailConfirmation">
///     Indicates whether the account policy blocks sign-in until the email address is confirmed.
/// </param>
public sealed record RegistrationResult(
    Guid UserId,
    UserActionTicket EmailConfirmation,
    bool RequiresEmailConfirmation);
