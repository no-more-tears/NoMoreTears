using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.DeactivateUser;

public sealed class DeactivateUserCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<DeactivateUserCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        DeactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(DeactivateUserCommand), UserRole.PlatformAdministrator);
        return Task.CompletedTask;
    }
}
