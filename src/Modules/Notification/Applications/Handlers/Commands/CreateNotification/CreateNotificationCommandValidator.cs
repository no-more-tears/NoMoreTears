using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;

public sealed class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(command => command.SourceEventId).RequiredId("Source event id");

        RuleFor(command => command.Type).Type();

        RuleFor(command => command.Data).NotNull().WithMessage("Notification data is required.");
    }
}
