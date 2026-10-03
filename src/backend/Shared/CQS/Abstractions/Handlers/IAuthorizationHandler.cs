using MediatR;

namespace NoMoreTears.Shared.CQS.Abstractions.Handlers;

public interface IAuthorizationHandler<in TRequest>
    where TRequest : IBaseRequest
{
    Task AuthorizeAsync(
        TRequest request, 
        CancellationToken cancellationToken);
}