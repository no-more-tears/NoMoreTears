using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(command => command.NotificationId).RequiredId("Notification id");
    }
}
