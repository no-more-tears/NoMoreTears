using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignUp;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;
using NoMoreTears.Shared.Providers.Identity.Diagnostics;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;

namespace NoMoreTears.Shared.Providers.Identity;

/// <summary>
///     Sign-up capability of <see cref="IdentityProvider"/>.
/// </summary>
internal sealed partial class IdentityProvider
{
    /// <inheritdoc />
    public async Task<RegistrationResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        EnsurePasswordPolicy(request.Password);

        var email = request.Email.Trim();
        var username = string.IsNullOrWhiteSpace(request.Username) ? email : request.Username.Trim();
        var normalizedEmail = Normalize(email);
        var normalizedUsername = Normalize(username);

        if (await IsIdentifierTakenAsync(normalizedEmail, normalizedUsername, cancellationToken))
        {
            throw new ConflictException(IdentityMessages.DuplicateUser);
        }

        var now = timeProvider.GetUtcNow();

        var user = new UserEntity
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            NormalizedEmail = normalizedEmail,
            Username = username,
            NormalizedUsername = normalizedUsername,
            PasswordHash = passwordHasher.HashPassword(request.Password),
            CreatedAt = now,
            PasswordChangedAt = now
        };

        user.Roles.Add(new UserRoleEntity { UserId = user.Id, Role = request.Role });
        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            // Lost a race with a concurrent registration of the same identifier.
            db.ChangeTracker.Clear();
            throw new ConflictException(IdentityMessages.DuplicateUser);
        }

        logger.LogInformation("User {UserId} registered.", user.Id);

        var ticket = await actionTokens.IssueAsync(user.Id, UserActionPurpose.EmailConfirmation, cancellationToken);
        return new RegistrationResult(user.Id, ticket, _account.RequireConfirmedEmail);
    }

    /// <inheritdoc />
    public async Task ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Token);

        var user = await FindUserAsync(request.UserId, cancellationToken)
            ?? throw new UnauthorizedException(IdentityMessages.InvalidToken);

        if (user.EmailConfirmed)
        {
            return;
        }

        if (!await actionTokens.ConsumeAsync(
                user.Id,
                UserActionPurpose.EmailConfirmation,
                request.Token,
                cancellationToken))
        {
            throw new UnauthorizedException(IdentityMessages.InvalidToken);
        }

        user.EmailConfirmed = true;
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserActionTicket?> RequestEmailConfirmationAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        var user = await FindUserAsync(identifier, cancellationToken);

        if (user is not { IsActive: true, EmailConfirmed: false })
        {
            return null;
        }

        return await actionTokens.TryIssueAsync(user.Id, UserActionPurpose.EmailConfirmation, cancellationToken);
    }

    /// <summary>
    ///     Checks both identifiers against both unique columns, so an email address
    ///     can never collide with somebody else's username at sign-in.
    /// </summary>
    private Task<bool> IsIdentifierTakenAsync(
        string normalizedEmail,
        string normalizedUsername,
        CancellationToken cancellationToken) =>
        db.Users.AnyAsync(
            x => x.NormalizedEmail == normalizedEmail
                || x.NormalizedEmail == normalizedUsername
                || x.NormalizedUsername == normalizedEmail
                || x.NormalizedUsername == normalizedUsername,
            cancellationToken);
}
