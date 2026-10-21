using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Tokens;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Persistence;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;
using NoMoreTears.Shared.Providers.Identity.Security;

namespace NoMoreTears.Shared.Providers.Identity.Tokens;

/// <summary>
///     EF Core backed implementation of <see cref="IMfaChallengeService"/>.
/// </summary>
internal sealed class MfaChallengeService(
    IdentityDbContext db,
    IOptions<MfaOptions> options,
    TimeProvider timeProvider) : IMfaChallengeService
{
    private const int ChallengeTokenSizeInBytes = 32;

    private readonly MfaOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<MfaChallenge> CreateAsync(
        Guid userId,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(client);

        var now = timeProvider.GetUtcNow();

        // A user has at most one open challenge.
        await db.MfaChallenges
            .Where(x => x.UserId == userId && x.ConsumedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ConsumedAt, now), cancellationToken);

        var challengeToken = TokenSecret.Generate(ChallengeTokenSizeInBytes);
        var code = TokenSecret.GenerateNumericCode(_options.CodeLength);
        var expiresAt = now + _options.ChallengeLifetime;

        db.MfaChallenges.Add(new MfaChallengeEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            ChallengeHash = TokenSecret.Hash(challengeToken),
            CodeHash = ComputeCodeHash(challengeToken, code),
            ClientFingerprint = ComputeFingerprint(client),
            CreatedAt = now,
            ExpiresAt = expiresAt
        });

        await db.SaveChangesAsync(cancellationToken);

        return new MfaChallenge(challengeToken, code, expiresAt);
    }

    /// <inheritdoc />
    public async Task<Guid?> VerifyAsync(
        string challengeToken,
        string code,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(challengeToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentNullException.ThrowIfNull(client);

        var now = timeProvider.GetUtcNow();
        var challengeHash = TokenSecret.Hash(challengeToken);

        var challenge = await db.MfaChallenges
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ChallengeHash == challengeHash, cancellationToken);

        if (challenge is null
            || challenge.ConsumedAt is not null
            || challenge.ExpiresAt <= now
            || challenge.FailedAttempts >= _options.MaxAttempts
            || !TokenSecret.FixedTimeEquals(challenge.ClientFingerprint, ComputeFingerprint(client)))
        {
            return null;
        }

        if (!TokenSecret.FixedTimeEquals(challenge.CodeHash, ComputeCodeHash(challengeToken, code)))
        {
            await db.MfaChallenges
                .Where(x => x.Id == challenge.Id && x.ConsumedAt == null)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(x => x.FailedAttempts, x => x.FailedAttempts + 1),
                    cancellationToken);

            return null;
        }

        // The conditional update guarantees a challenge is consumed by exactly one caller.
        var maxAttempts = _options.MaxAttempts;

        var consumed = await db.MfaChallenges
            .Where(x => x.Id == challenge.Id && x.ConsumedAt == null && x.FailedAttempts < maxAttempts)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.ConsumedAt, now), cancellationToken);

        return consumed == 1 ? challenge.UserId : null;
    }

    private static string ComputeCodeHash(string challengeToken, string code) =>
        TokenSecret.Hash(challengeToken + ":" + code);

    private static string ComputeFingerprint(ClientContext client) =>
        TokenSecret.Hash(client.UserAgent + "|" + client.DeviceId);
}
