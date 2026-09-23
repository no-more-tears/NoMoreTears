namespace NoMoreTears.Shared.Postgres.Abstractions.Repositories;

/// <summary>
///     Combined repository interface with CRUD, batch, and query capabilities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity ID.</typeparam>
public interface IRepository<TEntity, TId>
    : ICrudRepository<TEntity>, IBatchRepository<TEntity>, IQueryRepository<TEntity, TId>
    where TEntity : class
    where TId : struct;