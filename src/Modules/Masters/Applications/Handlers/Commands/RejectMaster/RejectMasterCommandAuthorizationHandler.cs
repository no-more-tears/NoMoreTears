using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMaster;

public sealed class RejectMasterCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<RejectMasterCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        RejectMasterCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(RejectMasterCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
