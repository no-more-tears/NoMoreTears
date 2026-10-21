using MediatR;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Shared.CQS.Behaviors;

/// <summary>
///     Pipeline behavior responsible for authorizing incoming requests.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class AuthorizationBehavior<TRequest, TResponse>(
    IEnumerable<IAuthorizationHandler<TRequest>> handlers,
    ICurrentUserSession currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    /// <summary>Collection of authorization handlers for the current request type.</summary>
    private readonly IReadOnlyList<IAuthorizationHandler<TRequest>> _handlers =
        handlers as IReadOnlyList<IAuthorizationHandler<TRequest>> ?? handlers.ToList();

    /// <summary>
    ///     Handles the request by executing all authorization handlers before pipeline execution.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_handlers.Count == 0)
        {
            return await next(cancellationToken);
        }

        // If unauthenticated session, reject before any handler runs.
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedException();
        }

        foreach (var handler in _handlers)
        {
            await handler.AuthorizeAsync(request, cancellationToken);
        }

        return await next(cancellationToken);
    }
}