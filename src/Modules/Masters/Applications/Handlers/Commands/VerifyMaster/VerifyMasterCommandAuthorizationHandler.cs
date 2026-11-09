using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMaster;

public sealed class VerifyMasterCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<VerifyMasterCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        VerifyMasterCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(VerifyMasterCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
