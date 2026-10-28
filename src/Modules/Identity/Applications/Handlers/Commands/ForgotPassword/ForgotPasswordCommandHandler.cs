using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService) : ICommandHandler<ForgotPasswordCommand>
{
    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        await identityService.ForgotPasswordAsync(
            request.Identifier,
            cancellationToken);
    }
}
