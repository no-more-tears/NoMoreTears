namespace NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

/// <summary>
///     A single-use token issued for a user action, ready to be delivered to the user.
/// </summary>
/// <remarks>
///     The provider never sends email itself; the host composes and delivers the message.
///     Only a hash of <paramref name="Token"/> is persisted, so the value is available once.
/// </remarks>
/// <param name="UserId">Identifier of the user the ticket was issued for.</param>
/// <param name="Email">Email address the ticket should be delivered to.</param>
/// <param name="Purpose">Action the ticket authorizes.</param>
/// <param name="Token">Plain token to include in the delivered message.</param>
/// <param name="ExpiresAt">Moment after which the token is rejected.</param>
public sealed record UserActionTicket(
    Guid UserId,
    string Email,
    UserActionPurpose Purpose,
    string Token,
    DateTimeOffset ExpiresAt);
