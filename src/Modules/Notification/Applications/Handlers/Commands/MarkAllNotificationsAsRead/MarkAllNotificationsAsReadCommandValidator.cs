using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.MarkAllNotificationsAsRead;

public sealed class MarkAllNotificationsAsReadCommandValidator : AbstractValidator<MarkAllNotificationsAsReadCommand>
{
    public MarkAllNotificationsAsReadCommandValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");
    }
}
