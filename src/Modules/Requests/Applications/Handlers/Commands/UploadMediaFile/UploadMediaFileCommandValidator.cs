using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.UploadMediaFile;

public sealed class UploadMediaFileCommandValidator : AbstractValidator<UploadMediaFileCommand>
{
    public UploadMediaFileCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.Content).NotNull().WithMessage("File is required.");

        RuleFor(command => command.FileName).FileName();

        RuleFor(command => command.ContentType).ContentType();

        RuleFor(command => command.SizeInBytes).FileSize();
    }
}