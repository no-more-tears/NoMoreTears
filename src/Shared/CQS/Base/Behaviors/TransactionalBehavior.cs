using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.CQS.Primitives.Commands;
using NoMoreTears.Shared.Postgres.Abstractions.Transactions;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for executing commands within the unit of work of their module.
/// </summary>
/// <remarks>
///     <para>
///         The <see cref="ITransactionalScope"/> is resolved as a keyed service by the assembly of the command,
///         so every module of a modular monolith works with its own DbContext. Commands of modules without
///         a registered scope run as is.
///     </para>
///     <para>
///         After the handler completes, pending changes are saved before the commit: handlers publish events
///         after services saved their data, and the transactional outbox stores those events in the context.
///     </para>
///     <para>
///         The constraint is <see cref="IBaseCommand"/>, not <c>IRequest&lt;TResponse&gt;</c>,
///         otherwise void commands would silently skip the behavior.
///     </para>
/// </remarks>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class TransactionalBehavior<TRequest, TResponse>(
    IServiceProvider serviceProvider)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IBaseCommand
{
    private const string ConcurrencyConflictMessage =
        "The resource was changed by another operation. Reload it and try again.";

    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var transactionalScope = serviceProvider.GetKeyedService<ITransactionalScope>(typeof(TRequest).Assembly);

        if (transactionalScope is null)
        {
            return await next(cancellationToken);
        }

        try
        {
            return transactionalScope.Mode is TransactionMode.SaveChanges || transactionalScope.HasActiveTransaction
                ? await SaveAfterAsync(transactionalScope, next, cancellationToken)
                : await ExecuteInTransactionAsync(transactionalScope, next, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(ConcurrencyConflictMessage);
        }
    }

    /// <summary>
    ///     Runs the command and saves pending changes; an ambient transaction (for example the one of a consumer
    ///     outbox) is committed by its owner.
    /// </summary>
    private static async Task<TResponse> SaveAfterAsync(
        ITransactionalScope transactionalScope,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken);
        await transactionalScope.SaveChangesAsync(cancellationToken);

        return response;
    }

    private static async Task<TResponse> ExecuteInTransactionAsync(
        ITransactionalScope transactionalScope,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var executionStrategy = transactionalScope.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await transactionalScope.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next(cancellationToken);

                await transactionalScope.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                if (transaction.IsActive)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                }

                throw;
            }
        });
    }
}
