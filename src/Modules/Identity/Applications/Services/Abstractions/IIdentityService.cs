using NoMoreTears.Modules.Identity.Applications.Services.Results;
using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Modules.Identity.Applications.Services.Abstractions;

public interface IIdentityService
{
    Task<RegistrationOutcome> RegisterAsync(
        string email,
        string? username,
        string password,
        CancellationToken cancellationToken);

    Task ConfirmEmailAsync(
        Guid userId, 
        string token,
        CancellationToken cancellationToken);

    Task<EmailConfirmationRequestedEvent?> ResendEmailConfirmationAsync(
        string identifier, 
        CancellationToken cancellationToken);

    Task<LoginOutcome> LoginAsync(
        string identifier,
        string password,
        ClientContext client,
        CancellationToken cancellationToken);

    Task<TokenResponse> VerifyMfaAsync(
        string challengeToken,
        string code,
        ClientContext client,
        CancellationToken cancellationToken);

    Task<TokenResponse> RefreshAsync(
        string refreshToken,
        ClientContext client,
        CancellationToken cancellationToken);

    Task LogoutAsync(
        string refreshToken, 
        CancellationToken cancellationToken);

    Task LogoutAllAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<PasswordResetRequestedEvent?> ForgotPasswordAsync(
        string identifier,
        CancellationToken cancellationToken);

    Task ResetPasswordAsync(
        Guid userId,
        string token,
        string newPassword,
        CancellationToken cancellationToken);
}
