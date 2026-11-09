using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectServiceArea;

public sealed class RejectServiceAreaCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<RejectServiceAreaCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        RejectServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(RejectServiceAreaCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
