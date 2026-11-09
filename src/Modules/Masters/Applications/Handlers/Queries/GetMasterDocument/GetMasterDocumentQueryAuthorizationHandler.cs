using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasterDocument;

public sealed class GetMasterDocumentQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<GetMasterDocumentQuery>(currentUser)
{
    public override Task AuthorizeAsync(
        GetMasterDocumentQuery request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(GetMasterDocumentQuery), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
