using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;
using NoMoreTears.Modules.Masters.Domain;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadAvatar;

public sealed class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    public UploadAvatarCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.Content).NotNull().WithMessage("File is required.");

        RuleFor(command => command.FileName).FileName();

        RuleFor(command => command.ContentType).ContentType();

        RuleFor(command => command.SizeInBytes).FileSize(MasterRules.MaxAvatarSizeInBytes);
    }
}
