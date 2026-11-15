namespace NoMoreTears.Modules.Requests.Applications.Services.Abstractions;

/// <summary>
///     Keeps the local snapshots of the Masters catalog and of the access of masters up to date.
/// </summary>
public interface IMastersSnapshotService
{
    Task SaveCategoryAsync(
        Guid categoryId,
        string name,
        DateTimeOffset changedAt,
        CancellationToken cancellationToken);

    Task ArchiveCategoryAsync(
        Guid categoryId,
        DateTimeOffset archivedAt,
        CancellationToken cancellationToken);

    Task ActivateCategoryAsync(
        Guid categoryId,
        DateTimeOffset activatedAt,
        CancellationToken cancellationToken);

    Task SaveMasterAccessAsync(
        Guid masterId,
        IReadOnlyCollection<Guid> acceptableCategoryIds,
        DateTimeOffset? suspendedUntil,
        DateTimeOffset changedAt,
        CancellationToken cancellationToken);
}