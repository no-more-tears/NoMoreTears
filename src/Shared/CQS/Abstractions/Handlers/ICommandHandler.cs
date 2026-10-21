using MediatR;
using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Shared.CQS.Abstractions.Handlers;

public interface ICommandHandler<in TCommand> 
    : IRequestHandler<TCommand>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> 
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;