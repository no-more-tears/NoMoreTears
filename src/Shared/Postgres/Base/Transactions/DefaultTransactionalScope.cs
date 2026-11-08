using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NoMoreTears.Shared.Postgres.Abstractions.Transactions;

namespace NoMoreTears.Shared.Postgres.Transactions;

/// <summary>
///     Manages the transactional boundary for the current <see cref="DbContext"/> unit of work.
///     Default implementation of <see cref="ITransactionalScope"/>.
/// </summary>
internal sealed class DefaultTransactionalScope : ITransactionalScope
{
    private readonly DbContext _dbContext;

    /// <inheritdoc />
    public bool HasActiveTransaction => _dbContext.Database.CurrentTransaction is not null;

    public DefaultTransactionalScope(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var database = _dbContext.Database;

        if (database.CurrentTransaction is IDbContextTransaction existing)
        {
            return new DefaultTransaction(existing);
        }

        var transaction = await database.BeginTransactionAsync(cancellationToken);
        return new DefaultTransaction(transaction);
    }

    /// <inheritdoc />
    public IExecutionStrategy CreateExecutionStrategy() =>
        _dbContext.Database.CreateExecutionStrategy();
}