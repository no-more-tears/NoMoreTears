using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Refresh;

public sealed class RefreshCommandHandler(
    IIdentityService identityService) : ICommandHandler<RefreshCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken)
    {
        return await identityService.RefreshAsync(
            request.RefreshToken,
            request.Context,
            cancellationToken);
    }
}
