using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotification;

public sealed class PushNotificationCommandValidator : AbstractValidator<PushNotificationCommand>
{
    public PushNotificationCommandValidator()
    {
        RuleFor(command => command.NotificationId).RequiredId("Notification id");
    }
}
