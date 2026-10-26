using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Register;

public sealed class RegisterCommandHandler(
    IIdentityService identityService) 
    : ICommandHandler<RegisterCommand, Guid>
{
    public async Task<Guid> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var userId = await identityService.RegisterAsync(
            request.Email,
            request.Username,
            request.Password,
            cancellationToken);

        return userId;
    }
}