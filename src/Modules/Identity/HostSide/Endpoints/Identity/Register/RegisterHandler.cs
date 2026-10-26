using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Register;
using NoMoreTears.Modules.Identity.Contracts.Requests;

namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity.Register;

public static class RegisterHandler
{
    public static async Task<IResult> Handle(
        [FromBody] RegisterRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email, 
            request.Username, 
            request.Password);
        var userId = await sender.Send(command, cancellationToken);

        return Results.Ok(userId);
    }
}