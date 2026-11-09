using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ReleaseVerificationHold;

public sealed class ReleaseVerificationHoldCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<ReleaseVerificationHoldCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        ReleaseVerificationHoldCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(ReleaseVerificationHoldCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
