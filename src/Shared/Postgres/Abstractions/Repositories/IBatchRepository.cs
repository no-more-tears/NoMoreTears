namespace NoMoreTears.Shared.Postgres.Abstractions.Repositories;

/// <summary>
///     A generic repository for batch operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IBatchRepository<TEntity>
{
    /// <summary>
    ///     Creates multiple new entities.
    /// </summary>
    void AddManyToBatch(TEntity[] entities);

    /// <summary>
    ///     Updates multiple existing entities.
    /// </summary>
    void UpdateManyInBatch(TEntity[] entities);

    /// <summary>
    ///     Deletes multiple existing entities.
    /// </summary>
    void DeleteManyFromBatch(TEntity[] entities);

    /// <summary>
    ///     Persists all changes made in the current batch.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}