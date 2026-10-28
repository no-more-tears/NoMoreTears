using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ResendEmailConfirmation;
using NoMoreTears.Modules.Identity.Contracts.Requests;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.ResendEmailConfirmation;

public static class ResendEmailConfirmationHandler
{
    public static async Task<IResult> Handle(
        [FromBody] ResendEmailConfirmationRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ResendEmailConfirmationCommand(
            request.Identifier);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}
