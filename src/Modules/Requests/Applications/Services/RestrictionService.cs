using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services;

/// <summary>
///     Applies publication restrictions of Trust to clients.
/// </summary>
public sealed class RestrictionService(
    IRepository<ClientRestriction, Guid> restrictions,
    TimeProvider timeProvider,
    ILogger<RestrictionService> logger) : IRestrictionService
{
    /// <inheritdoc />
    public async Task<bool> ApplyAsync(
        Guid restrictionId,
        Guid clientId,
        string kind,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<RestrictionKind>(kind, ignoreCase: true, out var restrictionKind) || !Enum.IsDefined(restrictionKind))
        {
            throw new BadRequestException($"Restriction kind '{kind}' is unknown.");
        }

        // Redelivered message.
        if (await restrictions.ExistsAsync(restriction => restriction.Id == restrictionId, cancellationToken))
        {
            return false;
        }

        var restriction = ClientRestriction.Create(
            restrictionId,
            clientId,
            restrictionKind,
            imposedAt,
            expiresAt,
            timeProvider.GetUtcNow());

        await restrictions.CreateAsync(restriction, cancellationToken);

        logger.LogInformation("Restriction {RestrictionId} of client {ClientId} applied", restrictionId, clientId);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> LiftAsync(
        Guid restrictionId,
        DateTimeOffset liftedAt,
        CancellationToken cancellationToken)
    {
        var restriction = await restrictions
            .AsQueryable()
            .FirstOrDefaultAsync(found => found.Id == restrictionId, cancellationToken);

        if (restriction is null || restriction.IsLifted)
        {
            return false;
        }

        restriction.Lift(liftedAt);
        await restrictions.UpdateAsync(restriction, cancellationToken);

        logger.LogInformation("Restriction {RestrictionId} of client {ClientId} lifted", restrictionId, restriction.ClientId);
        return true;
    }
}