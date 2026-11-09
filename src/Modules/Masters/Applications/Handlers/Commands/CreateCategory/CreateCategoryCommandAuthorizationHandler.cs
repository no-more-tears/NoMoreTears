using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateCategory;

public sealed class CreateCategoryCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<CreateCategoryCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(CreateCategoryCommand), UserRole.PlatformAdministrator);
        return Task.CompletedTask;
    }
}
