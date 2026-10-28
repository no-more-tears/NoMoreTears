using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandHandler(
    IIdentityService identityService) : ICommandHandler<ResendEmailConfirmationCommand>
{
    public async Task Handle(
        ResendEmailConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        await identityService.ResendEmailConfirmationAsync(
            request.Identifier,
            cancellationToken);
    }
}
