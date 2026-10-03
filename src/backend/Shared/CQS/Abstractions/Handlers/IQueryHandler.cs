using MediatR;
using NoMoreTears.Shared.CQS.Primitives.Queries;

namespace NoMoreTears.Shared.CQS.Abstractions.Handlers;

public interface IQueryHandler<in TQuery, TResponse> 
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;