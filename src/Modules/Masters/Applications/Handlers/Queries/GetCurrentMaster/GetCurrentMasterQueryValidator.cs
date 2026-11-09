using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCurrentMaster;

public sealed class GetCurrentMasterQueryValidator : AbstractValidator<GetCurrentMasterQuery>
{
    public GetCurrentMasterQueryValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
