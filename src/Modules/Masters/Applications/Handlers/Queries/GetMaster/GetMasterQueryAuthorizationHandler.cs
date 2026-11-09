using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMaster;

public sealed class GetMasterQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<GetMasterQuery>(currentUser)
{
    public override Task AuthorizeAsync(
        GetMasterQuery request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(GetMasterQuery), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
