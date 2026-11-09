using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyServiceArea;

public sealed class VerifyServiceAreaCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<VerifyServiceAreaCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        VerifyServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(VerifyServiceAreaCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
