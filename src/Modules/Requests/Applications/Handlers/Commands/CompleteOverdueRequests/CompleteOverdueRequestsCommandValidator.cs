using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CompleteOverdueRequests;

public sealed class CompleteOverdueRequestsCommandValidator : AbstractValidator<CompleteOverdueRequestsCommand>
{
    public CompleteOverdueRequestsCommandValidator()
    {
        RuleFor(command => command.BatchSize).GreaterThan(0).WithMessage("Batch size must be greater than 0.");
    }
}