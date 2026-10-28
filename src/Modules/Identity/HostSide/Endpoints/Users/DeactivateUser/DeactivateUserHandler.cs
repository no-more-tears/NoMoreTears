using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.DeactivateUser;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.DeactivateUser;

public static class DeactivateUserHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(userId);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
