using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateRoles;
using NoMoreTears.Modules.Identity.Contracts.Requests;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.UpdateRoles;

public static class UpdateRolesHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        [FromBody] UpdateRolesRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRolesCommand(
            userId,
            request.Roles);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
