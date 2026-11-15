using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.MarkRequestAsDone;

public sealed class MarkRequestAsDoneCommandValidator : AbstractValidator<MarkRequestAsDoneCommand>
{
    public MarkRequestAsDoneCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.RequestId).RequiredId("Request id");
    }
}