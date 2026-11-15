using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SettleDispute;

public sealed class SettleDisputeCommandValidator : AbstractValidator<SettleDisputeCommand>
{
    public SettleDisputeCommandValidator()
    {
        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.IncidentId).RequiredId("Incident id");
    }
}