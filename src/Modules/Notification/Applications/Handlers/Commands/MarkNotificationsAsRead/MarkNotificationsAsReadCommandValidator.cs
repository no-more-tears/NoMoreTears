using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationsAsRead;

public sealed class MarkNotificationsAsReadCommandValidator : AbstractValidator<MarkNotificationsAsReadCommand>
{
    public MarkNotificationsAsReadCommandValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(command => command.NotificationIds).NotificationIds();
    }
}
