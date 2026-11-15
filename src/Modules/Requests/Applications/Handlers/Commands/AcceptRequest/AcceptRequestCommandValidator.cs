using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.AcceptRequest;

public sealed class AcceptRequestCommandValidator : AbstractValidator<AcceptRequestCommand>
{
    public AcceptRequestCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.ArrivalIntervalInHours).ArrivalInterval();
    }
}