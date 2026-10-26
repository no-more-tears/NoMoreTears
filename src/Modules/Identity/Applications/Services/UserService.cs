using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Users;

namespace NoMoreTears.Modules.Identity.Applications.Services;

/// <inheritdoc />
public sealed class UserService : IUserService
{
    /// <inheritdoc />
    public Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, Guid? currentSessionId, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task DeactivateAsync(Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<UserInfo> GetAsync(Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IReadOnlyList<SessionInfo>> GetSessionsAsync(Guid userId, Guid? currentSessionId, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task ReactivateAsync(Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task RevokeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task UpdateMfaAsync(Guid userId, string password, bool enabled, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task UpdateRolesAsync(Guid userId, IReadOnlyCollection<UserRole> roles, CancellationToken cancellationToken) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task VerifyPasswordAsync(Guid userId, string password, CancellationToken cancellationToken) => throw new NotImplementedException();
}