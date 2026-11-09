using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasters;

public sealed class GetMastersQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<GetMastersQuery>(currentUser)
{
    public override Task AuthorizeAsync(
        GetMastersQuery request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(GetMastersQuery), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
