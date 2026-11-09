using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotificationsChange;

public sealed class PushNotificationsChangeCommandValidator : AbstractValidator<PushNotificationsChangeCommand>
{
    public PushNotificationsChangeCommandValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(command => command.Change).NotEmpty().WithMessage("Change is required.");
    }
}
