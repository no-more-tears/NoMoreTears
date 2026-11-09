using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.RecordEmailFailure;

public sealed class RecordEmailFailureCommandValidator : AbstractValidator<RecordEmailFailureCommand>
{
    public RecordEmailFailureCommandValidator()
    {
        RuleFor(command => command.SourceEventId).RequiredId("Source event id");

        RuleFor(command => command.RecipientId).RequiredId("Recipient id");
    }
}
