using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestDescription;

public sealed class ChangeRequestDescriptionCommandValidator : AbstractValidator<ChangeRequestDescriptionCommand>
{
    public ChangeRequestDescriptionCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.Description).ValidDescription();
    }
}