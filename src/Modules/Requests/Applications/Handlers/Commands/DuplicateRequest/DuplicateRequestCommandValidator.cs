using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DuplicateRequest;

public sealed class DuplicateRequestCommandValidator : AbstractValidator<DuplicateRequestCommand>
{
    public DuplicateRequestCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.Description).ValidDescription();
    }
}