using System.Linq.Expressions;

namespace NoMoreTears.Shared.Postgres.Abstractions.Repositories;

/// <summary>
///     A generic repository for query capabilities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity ID.</typeparam>
public interface IQueryRepository<TEntity, TId>
    where TEntity : class
    where TId : struct
{
    /// <summary>
    ///     Returns an <see cref="IQueryable{TEntity}"/> for building LINQ queries 
    ///     against the underlying data source.
    /// </summary>
    IQueryable<TEntity> AsQueryable();

    /// <summary>
    ///     Returns filtered <see cref="IQueryable{TEntity}"/> based on the provided predicate.
    /// </summary>
    IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    ///     Returns an entity by its identifier.
    /// </summary>
    Task<TEntity?> FindAsync(
        TId identifier,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns whether any entity exists that matches the provided predicate.
    /// </summary>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Returns the total count of entities.
    /// </summary>
    Task<long> CountAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Returns the total count of entities that match the provided predicate.
    /// </summary>
    Task<long> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Executes a raw SQL command.
    /// </summary>
    Task ExecuteRawSqlAsync(
        string sql,
        CancellationToken cancellationToken,
        params object[] parameters);
}