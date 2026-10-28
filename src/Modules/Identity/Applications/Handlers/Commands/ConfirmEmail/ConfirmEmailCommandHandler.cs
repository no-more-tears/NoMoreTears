using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IIdentityService identityService) : ICommandHandler<ConfirmEmailCommand>
{
    public async Task Handle(
        ConfirmEmailCommand request, 
        CancellationToken cancellationToken)
    {
        await identityService.ConfirmEmailAsync(
            request.UserId,
            request.Token,
            cancellationToken);
    }
}