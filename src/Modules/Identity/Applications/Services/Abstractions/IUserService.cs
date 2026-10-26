using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Users;

namespace NoMoreTears.Modules.Identity.Applications.Services.Abstractions;

public interface IUserService
{
    Task<UserInfo> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task VerifyPasswordAsync(
        Guid userId, 
        string password,
        CancellationToken cancellationToken);

    Task ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        Guid? currentSessionId,
        CancellationToken cancellationToken);

    Task UpdateMfaAsync(
        Guid userId,
        string password,
        bool enabled,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SessionInfo>> GetSessionsAsync(
        Guid userId, 
        Guid? currentSessionId,
        CancellationToken cancellationToken);

    Task RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken);

    Task UpdateRolesAsync(
        Guid userId,
        IReadOnlyCollection<UserRole> roles, 
        CancellationToken cancellationToken);

    Task DeactivateAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task ReactivateAsync(
        Guid userId,
        CancellationToken cancellationToken);
}