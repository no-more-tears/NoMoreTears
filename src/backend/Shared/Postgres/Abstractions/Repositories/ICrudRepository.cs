namespace NoMoreTears.Shared.Postgres.Abstractions.Repositories;

/// <summary>
///     A generic repository for CRUD operations with Unit of Work support.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface ICrudRepository<TEntity> 
    where TEntity : class
{
    /// <summary>
    ///     Creates a new entity. Invokes the Unit of Work to persist changes.
    /// </summary>
    /// <returns>The created entity.</returns>
    public Task<TEntity> CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Creates multiple new entities. Invokes the Unit of Work to persist changes.
    /// </summary>
    /// <returns>The created entities.</returns>
    public Task<List<TEntity>> CreateManyAsync(
        TEntity[] entities,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Updates an existing entity. Invokes the Unit of Work to persist changes.
    /// </summary>
    /// <remarks>
    ///     
    /// </remarks>
    public Task UpdateAsync(
        TEntity entity, 
        CancellationToken cancellationToken);

    /// <summary>
    ///     Updates multiple existing entities. Invokes the Unit of Work to persist changes.
    /// </summary>
    /// <remarks>
    ///     
    /// </remarks>
    public Task UpdateManyAsync(
        TEntity[] entities,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes an existing entity. Invokes the Unit of Work to persist changes.
    /// </summary>
    /// <returns>Indicates whether the entity was deleted.</returns>
    public Task<bool> DeleteAsync(
        TEntity entity, 
        CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes multiple existing entities. Invokes the Unit of Work to persist changes.
    /// </summary>
    /// <returns>Indicates whether the entities were deleted.</returns>
    public Task<bool> DeleteManyAsync(
        TEntity[] entities,
        CancellationToken cancellationToken);
}