using NoMoreTears.Shared.Providers.Identity.Contracts.Common;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;

namespace NoMoreTears.Modules.Identity.Applications.Services.Abstractions;

public interface IIdentityService
{
    Task<Guid> RegisterAsync(
        string email,
        string? username,
        string password,
        CancellationToken cancellationToken);

    Task ConfirmEmailAsync(
        Guid userId, 
        string token,
        CancellationToken cancellationToken);

    Task ResendEmailConfirmationAsync(
        string identifier, 
        CancellationToken cancellationToken);

    Task<LoginResult> LoginAsync(
        string identifier,
        string password,
        ClientContext client,
        CancellationToken cancellationToken);

    Task<TokenPair> VerifyMfaAsync(
        string challengeToken,
        string code,
        ClientContext client,
        CancellationToken cancellationToken);

    Task<TokenPair> RefreshAsync(
        string refreshToken,
        ClientContext client,
        CancellationToken cancellationToken);

    Task LogoutAsync(
        string refreshToken, 
        CancellationToken cancellationToken);

    Task LogoutAllAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task ForgotPasswordAsync(
        string identifier,
        CancellationToken cancellationToken);

    Task ResetPasswordAsync(
        Guid userId,
        string token,
        string newPassword,
        CancellationToken cancellationToken);

    Task<bool> IsSessionActiveAsync(
        Guid sessionId,
        CancellationToken cancellationToken);
}