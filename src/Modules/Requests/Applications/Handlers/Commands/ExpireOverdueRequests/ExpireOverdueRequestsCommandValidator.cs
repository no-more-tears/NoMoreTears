using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ExpireOverdueRequests;

public sealed class ExpireOverdueRequestsCommandValidator : AbstractValidator<ExpireOverdueRequestsCommand>
{
    public ExpireOverdueRequestsCommandValidator()
    {
        RuleFor(command => command.BatchSize).GreaterThan(0).WithMessage("Batch size must be greater than 0.");
    }
}