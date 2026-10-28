using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        return await identityService.LoginAsync(
            request.Identifier,
            request.Password,
            request.Context,
            cancellationToken);
    }
}