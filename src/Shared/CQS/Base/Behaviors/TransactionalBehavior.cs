using MediatR;
using Microsoft.EntityFrameworkCore;
using NoMoreTears.Shared.CQS.Primitives.Commands;
using NoMoreTears.Shared.Postgres.Abstractions.Transactions;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for executing requests within a database transaction.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class TransactionalBehavior<TRequest, TResponse>(
    ITransactionalScope transactionalScope)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IBaseCommand
{
    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (transactionalScope.HasActiveTransaction)
        {
            return await next(cancellationToken);
        }

        var executionStrategy = transactionalScope.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await transactionalScope.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}