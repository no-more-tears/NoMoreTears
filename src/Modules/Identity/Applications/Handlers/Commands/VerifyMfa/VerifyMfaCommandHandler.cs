using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyMfa;

public sealed class VerifyMfaCommandHandler(
    IIdentityService identityService) : ICommandHandler<VerifyMfaCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(
        VerifyMfaCommand request,
        CancellationToken cancellationToken)
    {
        return await identityService.VerifyMfaAsync(
            request.ChallengeToken,
            request.Code,
            request.Context,
            cancellationToken);
    }
}
