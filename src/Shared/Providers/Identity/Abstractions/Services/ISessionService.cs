using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Services;

/// <summary>
///     Lists, revokes and validates user sessions.
/// </summary>
/// <remarks>
///     A session is the lineage of rotated refresh tokens started by a single sign-in.
///     Access tokens carry the session identifier in the <c>sid</c> claim.
/// </remarks>
public interface ISessionService
{
    /// <summary>
    ///     Returns the active sessions of the user, most recently used first.
    /// </summary>
    /// <param name="userId">Identifier of the user.</param>
    /// <param name="currentSessionId">Session of the caller, used to mark the current entry.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<SessionInfo>> GetSessionsAsync(
        Guid userId,
        Guid? currentSessionId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Revokes a session of the user. Sessions of other users and unknown sessions are ignored.
    /// </summary>
    Task RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns whether the session exists, has not been revoked and has not expired.
    /// </summary>
    /// <remarks>
    ///     Intended for the bearer authentication pipeline to reject access tokens
    ///     of revoked sessions before they expire.
    /// </remarks>
    Task<bool> IsSessionActiveAsync(
        Guid sessionId,
        CancellationToken cancellationToken);
}