using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Mappings;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Providers.Identity.Abstractions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;
using NoMoreTears.Shared.Providers.Identity.Contracts.Users;

namespace NoMoreTears.Modules.Identity.Applications.Services;

/// <summary>
///     Provides account self-service and user administration functionality.
/// </summary>
public sealed class UserService(
    IIdentityProvider identityProvider,
    ILogger<UserService> logger) : IUserService
{
    private const string UserNotFoundMessage = "User not found.";

    /// <inheritdoc />
    public async Task<UserResponse> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await identityProvider.GetUserAsync(userId, cancellationToken)
            ?? throw new NotFoundException(UserNotFoundMessage);

        return user.ToResponse();
    }

    /// <inheritdoc />
    public async Task VerifyPasswordAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken)
    {
        await identityProvider.VerifyPasswordAsync(userId, password, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        Guid? currentSessionId,
        CancellationToken cancellationToken)
    {
        var changePasswordRequest = new ChangePasswordRequest(userId, currentPassword, newPassword, currentSessionId);
        await identityProvider.ChangePasswordAsync(changePasswordRequest, cancellationToken);

        logger.LogInformation("User {UserId} changed password", userId);
    }

    /// <inheritdoc />
    public async Task UpdateMfaAsync(
        Guid userId,
        string password,
        bool enabled,
        CancellationToken cancellationToken)
    {
        var updateMfaRequest = new UpdateMfaRequest(userId, password, enabled);
        await identityProvider.UpdateMfaAsync(updateMfaRequest, cancellationToken);

        logger.LogInformation("User {UserId} set MFA enabled to {MfaEnabled}", userId, enabled);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SessionResponse>> GetSessionsAsync(
        Guid userId,
        Guid? currentSessionId,
        CancellationToken cancellationToken)
    {
        var sessions = await identityProvider.GetSessionsAsync(userId, currentSessionId, cancellationToken);

        return sessions
            .Select(session => session.ToResponse())
            .ToList();
    }

    /// <inheritdoc />
    public async Task RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        await identityProvider.RevokeSessionAsync(userId, sessionId, cancellationToken);

        logger.LogInformation("User {UserId} revoked session {SessionId}", userId, sessionId);
    }

    /// <inheritdoc />
    public async Task UpdateRolesAsync(
        Guid userId,
        IReadOnlyCollection<UserRole> roles,
        CancellationToken cancellationToken)
    {
        await identityProvider.UpdateRolesAsync(userId, roles, cancellationToken);

        logger.LogInformation("Roles of user {UserId} updated to {Roles}", userId, roles);
    }

    /// <inheritdoc />
    public async Task DeactivateAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        await identityProvider.DeactivateAsync(userId, cancellationToken);

        logger.LogInformation("User {UserId} deactivated", userId);
    }

    /// <inheritdoc />
    public async Task ReactivateAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        await identityProvider.ReactivateAsync(userId, cancellationToken);

        logger.LogInformation("User {UserId} reactivated", userId);
    }
}