using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUndeliveredNotifications;

public sealed class GetUndeliveredNotificationsQueryValidator : AbstractValidator<GetUndeliveredNotificationsQuery>
{
    public GetUndeliveredNotificationsQueryValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");
    }
}
