using MediatR;

namespace NoMoreTears.Shared.CQS.Primitives.Commands;

public interface IBaseCommand;

public interface ICommand : IBaseCommand, IRequest;

public interface ICommand<TResponse> : IBaseCommand, IRequest<TResponse>;