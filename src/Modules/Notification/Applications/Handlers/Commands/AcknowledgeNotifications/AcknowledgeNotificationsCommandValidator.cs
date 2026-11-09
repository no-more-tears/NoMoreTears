using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.AcknowledgeNotifications;

public sealed class AcknowledgeNotificationsCommandValidator : AbstractValidator<AcknowledgeNotificationsCommand>
{
    public AcknowledgeNotificationsCommandValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(command => command.NotificationIds).NotificationIds();
    }
}
