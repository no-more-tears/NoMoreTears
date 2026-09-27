using MediatR;

namespace NoMoreTears.Shared.CQS.Primitives.Queries;

public interface IQuery<TResponse> : IRequest<TResponse>;