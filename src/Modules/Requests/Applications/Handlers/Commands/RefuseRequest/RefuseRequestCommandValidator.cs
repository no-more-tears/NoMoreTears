using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RefuseRequest;

public sealed class RefuseRequestCommandValidator : AbstractValidator<RefuseRequestCommand>
{
    public RefuseRequestCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.Reason).RequiredReason();
    }
}