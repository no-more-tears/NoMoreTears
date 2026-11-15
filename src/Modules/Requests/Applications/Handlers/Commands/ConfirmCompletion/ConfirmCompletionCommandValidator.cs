using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ConfirmCompletion;

public sealed class ConfirmCompletionCommandValidator : AbstractValidator<ConfirmCompletionCommand>
{
    public ConfirmCompletionCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");
    }
}