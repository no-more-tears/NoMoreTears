using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ObjectToCompletion;

public sealed class ObjectToCompletionCommandValidator : AbstractValidator<ObjectToCompletionCommand>
{
    public ObjectToCompletionCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.Reason).RequiredReason();
    }
}