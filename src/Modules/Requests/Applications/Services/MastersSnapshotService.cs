using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services;

/// <summary>
///     Keeps the local snapshots of the Masters catalog and of the access of masters up to date (anti-corruption layer).
///     Events may arrive out of order; the snapshots ignore states older than the stored one.
/// </summary>
public sealed class MastersSnapshotService(
    IRepository<ServiceCategory, Guid> categories,
    IRepository<MasterAccess, Guid> masterAccesses,
    ILogger<MastersSnapshotService> logger) : IMastersSnapshotService
{
    /// <inheritdoc />
    public async Task SaveCategoryAsync(
        Guid categoryId,
        string name,
        DateTimeOffset changedAt,
        CancellationToken cancellationToken)
    {
        var category = await FindCategoryAsync(categoryId, cancellationToken);

        if (category is null)
        {
            await categories.CreateAsync(ServiceCategory.Create(categoryId, name, changedAt), cancellationToken);
        }
        else if (category.Rename(name, changedAt))
        {
            await categories.UpdateAsync(category, cancellationToken);
        }

        logger.LogInformation("Category {CategoryId} of the catalog snapshot saved", categoryId);
    }

    /// <inheritdoc />
    public async Task ArchiveCategoryAsync(
        Guid categoryId,
        DateTimeOffset archivedAt,
        CancellationToken cancellationToken)
    {
        var category = await FindCategoryAsync(categoryId, cancellationToken);

        if (category is not null && category.Archive(archivedAt))
        {
            await categories.UpdateAsync(category, cancellationToken);
            logger.LogInformation("Category {CategoryId} of the catalog snapshot archived", categoryId);
        }
    }

    /// <inheritdoc />
    public async Task ActivateCategoryAsync(
        Guid categoryId,
        DateTimeOffset activatedAt,
        CancellationToken cancellationToken)
    {
        var category = await FindCategoryAsync(categoryId, cancellationToken);

        if (category is not null && category.Activate(activatedAt))
        {
            await categories.UpdateAsync(category, cancellationToken);
            logger.LogInformation("Category {CategoryId} of the catalog snapshot activated", categoryId);
        }
    }

    /// <inheritdoc />
    public async Task SaveMasterAccessAsync(
        Guid masterId,
        IReadOnlyCollection<Guid> acceptableCategoryIds,
        DateTimeOffset? suspendedUntil,
        DateTimeOffset changedAt,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(acceptableCategoryIds);

        var access = await masterAccesses
            .AsQueryable()
            .FirstOrDefaultAsync(found => found.Id == masterId, cancellationToken);

        if (access is null)
        {
            await masterAccesses.CreateAsync(
                MasterAccess.Create(masterId, acceptableCategoryIds, suspendedUntil, changedAt),
                cancellationToken);
        }
        else if (access.Refresh(acceptableCategoryIds, suspendedUntil, changedAt))
        {
            await masterAccesses.UpdateAsync(access, cancellationToken);
        }

        logger.LogInformation("Access snapshot of master {MasterId} saved", masterId);
    }

    private Task<ServiceCategory?> FindCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        return categories
            .AsQueryable()
            .FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken);
    }
}