using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RepublishRequest;

public sealed class RepublishRequestCommandValidator : AbstractValidator<RepublishRequestCommand>
{
    public RepublishRequestCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");
    }
}