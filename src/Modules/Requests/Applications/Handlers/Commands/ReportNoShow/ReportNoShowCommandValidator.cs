using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ReportNoShow;

public sealed class ReportNoShowCommandValidator : AbstractValidator<ReportNoShowCommand>
{
    public ReportNoShowCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");
    }
}