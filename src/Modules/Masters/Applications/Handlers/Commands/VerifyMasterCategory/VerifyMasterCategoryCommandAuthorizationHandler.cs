using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMasterCategory;

public sealed class VerifyMasterCategoryCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<VerifyMasterCategoryCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        VerifyMasterCategoryCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(VerifyMasterCategoryCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
