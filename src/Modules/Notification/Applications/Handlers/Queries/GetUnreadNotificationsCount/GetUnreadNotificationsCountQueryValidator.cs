using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUnreadNotificationsCount;

public sealed class GetUnreadNotificationsCountQueryValidator : AbstractValidator<GetUnreadNotificationsCountQuery>
{
    public GetUnreadNotificationsCountQueryValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");
    }
}
