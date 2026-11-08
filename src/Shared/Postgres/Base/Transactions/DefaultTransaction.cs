using Microsoft.EntityFrameworkCore.Storage;
using NoMoreTears.Shared.Postgres.Abstractions.Transactions;

namespace NoMoreTears.Shared.Postgres.Transactions;

/// <summary>
///     Wraps an <see cref="IDbContextTransaction"/> and exposes it through the <see cref="ITransaction"/> contract.
///     Default implementation of <see cref="ITransaction"/>.
/// </summary>
public sealed class DefaultTransaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;
    private bool _disposed;

    /// <inheritdoc />
    public Guid TransactionId { get; }

    /// <inheritdoc />
    public bool IsActive { get => field && !_disposed; private set; } = true;

    public DefaultTransaction(IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        _transaction = transaction;
        TransactionId = transaction.TransactionId;
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotActive();

        await _transaction.CommitAsync(cancellationToken);
        IsActive = false;
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotActive();

        await _transaction.RollbackAsync(cancellationToken);
        IsActive = false;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await _transaction.DisposeAsync();
    }

    private void ThrowIfNotActive()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException(
                $"Transaction '{TransactionId}' is no longer active (already committed, rolled back, or disposed).");
        }
    }
}