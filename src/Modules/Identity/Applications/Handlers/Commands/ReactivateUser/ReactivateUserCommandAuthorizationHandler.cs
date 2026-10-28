using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ReactivateUser;

public sealed class ReactivateUserCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<ReactivateUserCommand>(currentUser)
{
    public override Task AuthorizeAsync(
        ReactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(ReactivateUserCommand), UserRole.PlatformAdministrator);
        return Task.CompletedTask;
    }
}
