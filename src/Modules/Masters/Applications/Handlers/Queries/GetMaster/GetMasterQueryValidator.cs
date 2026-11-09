using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMaster;

public sealed class GetMasterQueryValidator : AbstractValidator<GetMasterQuery>
{
    public GetMasterQueryValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
