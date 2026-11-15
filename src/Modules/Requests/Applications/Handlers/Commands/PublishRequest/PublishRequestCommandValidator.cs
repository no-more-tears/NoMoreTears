using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.PublishRequest;

public sealed class PublishRequestCommandValidator : AbstractValidator<PublishRequestCommand>
{
    public PublishRequestCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");
    }
}