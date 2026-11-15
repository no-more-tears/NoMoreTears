using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestCategory;

public sealed class ChangeRequestCategoryCommandValidator : AbstractValidator<ChangeRequestCategoryCommand>
{
    public ChangeRequestCategoryCommandValidator()
    {
        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.RequestId).RequiredId("Request id");

        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}