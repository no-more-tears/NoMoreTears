using NoMoreTears.Shared.Providers.Identity.Contracts.SignUp;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Services;

/// <summary>
///     Registers users and confirms their email addresses.
/// </summary>
public interface ISignUpService
{
    /// <summary>
    ///     Creates a user with the initial role and issues an email confirmation ticket.
    /// </summary>
    /// <returns>Identifier of the user and the ticket to deliver.</returns>
    Task<RegistrationResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Confirms the email address with a single-use token. Succeeds silently
    ///     when the address was already confirmed.
    /// </summary>
    Task ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Issues a fresh email confirmation ticket, invalidating previously issued ones.
    /// </summary>
    /// <remarks>
    ///     Returns <see langword="null"/> when no active user with an unconfirmed address
    ///     matches the identifier, so callers cannot probe for existing accounts.
    /// </remarks>
    Task<UserActionTicket?> RequestEmailConfirmationAsync(
        string identifier,
        CancellationToken cancellationToken);
}