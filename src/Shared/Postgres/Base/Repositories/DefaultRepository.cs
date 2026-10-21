using Microsoft.EntityFrameworkCore;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NoMoreTears.Shared.Postgres.Repositories;

/// <summary>
///     A default implementation of combined Repository.
/// </summary>
public sealed class DefaultRepository<TEntity, TId> 
    : IRepository<TEntity, TId>
    where TEntity : class
    where TId : struct
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public DefaultRepository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable();

    /// <inheritdoc />
    public async Task<TEntity> CreateAsync(
        TEntity entity, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return entity;
    }

    /// <inheritdoc />
    public async Task<List<TEntity>> CreateManyAsync(
        TEntity[] entities,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return [];
        }

        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return entities.ToList();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = _dbContext.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        entry.State = EntityState.Modified;
        await SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateManyAsync(
        TEntity[] entities,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return;
        }

        foreach (var entity in entities)
        {
            var entry = _dbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        await SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        TEntity entity, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Remove(entity);
        var affected = await SaveChangesAsync(cancellationToken);

        return affected > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteManyAsync(
        TEntity[] entities,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return false;
        }

        _dbSet.RemoveRange(entities);
        var affected = await SaveChangesAsync(cancellationToken);

        return affected > 0;
    }

    /// <inheritdoc />
    public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _dbSet.Where(predicate);
    }

    /// <inheritdoc />
    public async Task<TEntity?> FindAsync(
        TId identifier,
        CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([identifier], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> CountAsync(CancellationToken cancellationToken) =>
        await _dbSet.LongCountAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<long> CountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await _dbSet.LongCountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ExecuteRawSqlAsync(
        string sql,
        CancellationToken cancellationToken,
        params object[] parameters)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(sql);
        await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddManyToBatch(TEntity[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbSet.AddRange(entities);
    }

    /// <inheritdoc />
    public void UpdateManyInBatch(TEntity[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            var entry = _dbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeleteManyFromBatch(TEntity[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbSet.RemoveRange(entities);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
} 