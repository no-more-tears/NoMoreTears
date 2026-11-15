using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestAddress;

public sealed class ChangeRequestAddressCommandValidator : AbstractValidator<ChangeRequestAddressCommand>
{
    public ChangeRequestAddressCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.Address).ValidAddress();
    }
}