using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.SubmitForVerification;

public sealed class SubmitForVerificationCommandValidator : AbstractValidator<SubmitForVerificationCommand>
{
    public SubmitForVerificationCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
