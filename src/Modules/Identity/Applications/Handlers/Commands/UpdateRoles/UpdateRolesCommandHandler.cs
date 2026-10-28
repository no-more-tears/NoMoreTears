using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateRoles;

public sealed class UpdateRolesCommandHandler(
    IUserService userService) : ICommandHandler<UpdateRolesCommand>
{
    public async Task Handle(
        UpdateRolesCommand request,
        CancellationToken cancellationToken)
    {
        await userService.UpdateRolesAsync(
            request.UserId,
            request.Roles,
            cancellationToken);
    }
}
