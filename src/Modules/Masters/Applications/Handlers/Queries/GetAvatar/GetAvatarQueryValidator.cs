using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetAvatar;

public sealed class GetAvatarQueryValidator : AbstractValidator<GetAvatarQuery>
{
    public GetAvatarQueryValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
