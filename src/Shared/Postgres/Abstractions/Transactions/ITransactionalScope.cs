using Microsoft.EntityFrameworkCore.Storage;

namespace NoMoreTears.Shared.Postgres.Abstractions.Transactions;

/// <summary>
///     Manages the transactional boundary for the current unit of work.
///     Wraps an ambient <see cref="DbContext"/> transaction,
///     ensuring that a single active transaction is reused rather than nested.
/// </summary>
public interface ITransactionalScope
{
    /// <summary>
    ///     Indicates whether a transaction is currently open on the underlying <see cref="DbContext"/>.
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    ///     Begins a new database transaction, or returns a wrapper over the already-active one
    ///     if a transaction is already open on the underlying <see cref="DbContext"/>.
    /// </summary>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Creates the execution strategy configured for the underlying <see cref="DbContext"/>.
    /// </summary>
    IExecutionStrategy CreateExecutionStrategy();
}