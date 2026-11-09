using FluentValidation;
using NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetNotifications;

public sealed class GetNotificationsQueryValidator : AbstractValidator<GetNotificationsQuery>
{
    public GetNotificationsQueryValidator()
    {
        RuleFor(command => command.RecipientId).RequiredId("Recipient id");

        RuleFor(query => query.PageNumber).PageNumber();

        RuleFor(query => query.PageSize).PageSize();
    }
}
