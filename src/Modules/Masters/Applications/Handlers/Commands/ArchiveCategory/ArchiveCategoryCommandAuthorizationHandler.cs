using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ArchiveCategory;

public sealed class ArchiveCategoryCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<ArchiveCategoryCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        ArchiveCategoryCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(ArchiveCategoryCommand), UserRole.PlatformAdministrator);
        return Task.CompletedTask;
    }
}
