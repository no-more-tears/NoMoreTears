using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Tokens;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;
using NoMoreTears.Shared.Providers.Identity.Diagnostics;
using NoMoreTears.Shared.Providers.Identity.Persistence;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;
using NoMoreTears.Shared.Providers.Identity.Security;

namespace NoMoreTears.Shared.Providers.Identity.Tokens;

/// <summary>
///     EF Core backed implementation of <see cref="IUserActionTokenService"/>.
/// </summary>
internal sealed class UserActionTokenService(
    IdentityDbContext db,
    IOptions<UserActionTokenOptions> options,
    TimeProvider timeProvider) : IUserActionTokenService
{
    private const int TokenSizeInBytes = 32;

    private readonly UserActionTokenOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<UserActionTicket> IssueAsync(
        Guid userId,
        UserActionPurpose purpose,
        CancellationToken cancellationToken)
    {
        var recipient = await db.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new { x.Id, x.Email })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(IdentityMessages.UserNotFound);

        var now = timeProvider.GetUtcNow();

        // Only the most recently issued token of a purpose stays valid.
        await db.UserActionTokens
            .Where(x => x.UserId == userId && x.Purpose == purpose && x.ConsumedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ConsumedAt, now), cancellationToken);

        var token = TokenSecret.Generate(TokenSizeInBytes);
        var expiresAt = now + GetLifetime(purpose);

        db.UserActionTokens.Add(new UserActionTokenEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Purpose = purpose,
            TokenHash = TokenSecret.Hash(token),
            CreatedAt = now,
            ExpiresAt = expiresAt
        });

        await db.SaveChangesAsync(cancellationToken);

        return new UserActionTicket(recipient.Id, recipient.Email, purpose, token, expiresAt);
    }

    /// <inheritdoc />
    public async Task<UserActionTicket?> TryIssueAsync(
        Guid userId,
        UserActionPurpose purpose,
        CancellationToken cancellationToken)
    {
        var issuedAfter = timeProvider.GetUtcNow() - _options.ReissueCooldown;

        // Superseded tokens keep their creation time until the purge, so they still count.
        var coolingDown = await db.UserActionTokens.AnyAsync(
            x => x.UserId == userId && x.Purpose == purpose && x.CreatedAt > issuedAfter,
            cancellationToken);

        return coolingDown
            ? null
            : await IssueAsync(userId, purpose, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ConsumeAsync(
        Guid userId,
        UserActionPurpose purpose,
        string token,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var now = timeProvider.GetUtcNow();
        var tokenHash = TokenSecret.Hash(token);

        var consumed = await db.UserActionTokens
            .Where(x => x.UserId == userId
                && x.Purpose == purpose
                && x.TokenHash == tokenHash
                && x.ConsumedAt == null
                && x.ExpiresAt > now)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ConsumedAt, now), cancellationToken);

        return consumed == 1;
    }

    private TimeSpan GetLifetime(UserActionPurpose purpose) =>
        purpose switch
        {
            UserActionPurpose.EmailConfirmation => _options.EmailConfirmationLifetime,
            UserActionPurpose.PasswordReset => _options.PasswordResetLifetime,
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, "Unsupported user action purpose.")
        };
}
