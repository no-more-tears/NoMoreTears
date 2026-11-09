using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ActivateCategory;

public sealed class ActivateCategoryCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<ActivateCategoryCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        ActivateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(ActivateCategoryCommand), UserRole.PlatformAdministrator);
        return Task.CompletedTask;
    }
}
