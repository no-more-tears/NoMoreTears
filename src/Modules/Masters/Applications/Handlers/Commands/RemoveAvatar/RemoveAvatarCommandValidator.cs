using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveAvatar;

public sealed class RemoveAvatarCommandValidator : AbstractValidator<RemoveAvatarCommand>
{
    public RemoveAvatarCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
