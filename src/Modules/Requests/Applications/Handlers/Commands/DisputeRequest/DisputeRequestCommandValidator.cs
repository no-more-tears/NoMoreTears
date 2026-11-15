using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DisputeRequest;

public sealed class DisputeRequestCommandValidator : AbstractValidator<DisputeRequestCommand>
{
    public DisputeRequestCommandValidator()
    {
        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.IncidentId).RequiredId("Incident id");
    }
}