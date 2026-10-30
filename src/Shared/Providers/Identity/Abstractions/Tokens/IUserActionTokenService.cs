using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Tokens;

/// <summary>
///     Issues and consumes single-use tokens for email confirmation and password reset.
/// </summary>
/// <remarks>
///     Only hashes are persisted; the plain token is available once in the issued ticket.
/// </remarks>
public interface IUserActionTokenService
{
    /// <summary>
    ///     Issues a token for the purpose, invalidating outstanding tokens of the same purpose.
    /// </summary>
    Task<UserActionTicket> IssueAsync(
        Guid userId,
        UserActionPurpose purpose,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Issues a token like <see cref="IssueAsync"/> unless a token of the same purpose
    ///     was issued within the reissue cooldown, so a mailbox cannot be flooded.
    /// </summary>
    /// <returns>The ticket, or <see langword="null"/> while the cooldown lasts.</returns>
    Task<UserActionTicket?> TryIssueAsync(
        Guid userId,
        UserActionPurpose purpose,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Atomically consumes the token when it belongs to the user, serves the purpose,
    ///     has not been consumed and has not expired.
    /// </summary>
    /// <returns>Whether the token was consumed.</returns>
    Task<bool> ConsumeAsync(
        Guid userId,
        UserActionPurpose purpose,
        string token,
        CancellationToken cancellationToken);
}