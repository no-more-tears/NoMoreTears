using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMasterCategory;

public sealed class RejectMasterCategoryCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<RejectMasterCategoryCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        RejectMasterCategoryCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(RejectMasterCategoryCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
