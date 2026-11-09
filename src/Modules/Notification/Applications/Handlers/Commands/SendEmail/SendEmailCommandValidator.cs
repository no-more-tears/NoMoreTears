using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.SendEmail;

public sealed class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(command => command.SourceEventId).RequiredId("Source event id");

        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(command => command.Address).NotEmpty().WithMessage("Email address is required.");

        RuleFor(command => command.Template).NotEmpty().WithMessage("Email template is required.");
    }
}
