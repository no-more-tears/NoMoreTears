using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.PlaceVerificationHold;

public sealed class PlaceVerificationHoldCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<PlaceVerificationHoldCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        PlaceVerificationHoldCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(PlaceVerificationHoldCommand), UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}
