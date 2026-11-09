using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<UpdateCategoryCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(UpdateCategoryCommand), UserRole.PlatformAdministrator);
        return Task.CompletedTask;
    }
}
