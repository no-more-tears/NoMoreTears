using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.DeleteNotification;

public sealed class DeleteNotificationCommandValidator : AbstractValidator<DeleteNotificationCommand>
{
    public DeleteNotificationCommandValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(command => command.NotificationId).RequiredId("Notification id");
    }
}
