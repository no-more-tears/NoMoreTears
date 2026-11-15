using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DiscardRequest;

public sealed class DiscardRequestCommandValidator : AbstractValidator<DiscardRequestCommand>
{
    public DiscardRequestCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");
    }
}