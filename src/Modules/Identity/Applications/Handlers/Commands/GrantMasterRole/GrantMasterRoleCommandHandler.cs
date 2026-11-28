using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.GrantMasterRole;

public sealed class GrantMasterRoleCommandHandler(
    IUserService userService) : ICommandHandler<GrantMasterRoleCommand>
{
    public async Task Handle(
        GrantMasterRoleCommand request,
        CancellationToken cancellationToken)
    {
        await userService.GrantRoleAsync(
            request.UserId,
            UserRole.Master,
            cancellationToken);
    }
}
