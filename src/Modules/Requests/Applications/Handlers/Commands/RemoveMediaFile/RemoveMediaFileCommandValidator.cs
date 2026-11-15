using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RemoveMediaFile;

public sealed class RemoveMediaFileCommandValidator : AbstractValidator<RemoveMediaFileCommand>
{
    public RemoveMediaFileCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.MediaFileId).RequiredId("Media file id");
    }
}