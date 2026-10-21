using Microsoft.EntityFrameworkCore;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;
using NoMoreTears.Shared.Providers.Identity.Contracts.Sessions;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Users;
using NoMoreTears.Shared.Providers.Identity.Diagnostics;

namespace NoMoreTears.Shared.Providers.Identity;

/// <summary>
///     Account self-service capability of <see cref="IdentityProvider"/>.
/// </summary>
internal sealed partial class IdentityProvider
{
    /// <inheritdoc />
    public async Task<UserInfo?> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user is null
            ? null
            : new UserInfo(
                user.Id,
                user.Email,
                user.Username,
                user.EmailConfirmed,
                user.IsActive,
                user.MfaEnabled,
                user.Roles.Select(x => x.Role).ToArray(),
                user.CreatedAt,
                user.LastLoginAt);
    }

    /// <inheritdoc />
    public async Task VerifyPasswordAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var passwordHash = await db.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.PasswordHash)
            .SingleOrDefaultAsync(cancellationToken);

        if (VerifyPassword(password, passwordHash) == PasswordVerificationStatus.Failed)
        {
            throw new UnauthorizedException(IdentityMessages.InvalidCredentials);
        }
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(request.CurrentPassword);
        EnsurePasswordPolicy(request.NewPassword);

        var user = await FindUserAsync(request.UserId, cancellationToken);
        var verification = VerifyPassword(request.CurrentPassword, user?.PasswordHash);

        if (user is null || verification == PasswordVerificationStatus.Failed)
        {
            throw new UnauthorizedException(IdentityMessages.InvalidCredentials);
        }

        var now = timeProvider.GetUtcNow();

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordChangedAt = now;
        await db.SaveChangesAsync(cancellationToken);

        await RevokeSessionsAsync(
            user.Id,
            SessionRevocationReason.PasswordChanged,
            now,
            request.CurrentSessionId,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserActionTicket?> ForgotPasswordAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        var user = await FindUserAsync(identifier, cancellationToken);

        if (user is not { IsActive: true })
        {
            return null;
        }

        return await actionTokens.IssueAsync(user.Id, UserActionPurpose.PasswordReset, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Token);
        EnsurePasswordPolicy(request.NewPassword);

        if (!await actionTokens.ConsumeAsync(
                request.UserId,
                UserActionPurpose.PasswordReset,
                request.Token,
                cancellationToken))
        {
            throw new UnauthorizedException(IdentityMessages.InvalidToken);
        }

        var user = await FindUserAsync(request.UserId, cancellationToken)
            ?? throw new UnauthorizedException(IdentityMessages.InvalidToken);

        var now = timeProvider.GetUtcNow();

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.PasswordChangedAt = now;
        // The token reached the inbox, which proves ownership of the address.
        user.EmailConfirmed = true;
        user.FailedAccessAttempts = 0;
        user.LockoutEndAt = null;
        await db.SaveChangesAsync(cancellationToken);

        await RevokeSessionsAsync(
            user.Id,
            SessionRevocationReason.PasswordReset,
            now,
            exceptSessionId: null,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateMfaAsync(
        UpdateMfaRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(request.Password);

        var user = await FindUserAsync(request.UserId, cancellationToken);
        var verification = VerifyPassword(request.Password, user?.PasswordHash);

        if (user is null || verification == PasswordVerificationStatus.Failed)
        {
            throw new UnauthorizedException(IdentityMessages.InvalidCredentials);
        }
        if (request.Enabled && !user.EmailConfirmed)
        {
            throw new ForbiddenException(IdentityMessages.MfaRequiresConfirmedEmail);
        }
        if (user.MfaEnabled == request.Enabled)
        {
            return;
        }

        user.MfaEnabled = request.Enabled;
        await db.SaveChangesAsync(cancellationToken);
    }
}