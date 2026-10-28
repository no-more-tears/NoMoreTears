using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ReactivateUser;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users.ReactivateUser;

public static class ReactivateUserHandler
{
    public static async Task<IResult> Handle(
        [FromRoute] Guid userId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ReactivateUserCommand(userId);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
