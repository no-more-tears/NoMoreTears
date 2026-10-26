using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.Providers.Identity.Abstractions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignUp;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;

namespace NoMoreTears.Modules.Identity.Applications.Services;

/// <summary>
///     Provides identity management functionality.
/// </summary>
public sealed class IdentityService(
    IIdentityProvider identityProvider,
    IIdentityNotifier identityNotifier,
    ILogger<IdentityService> logger) : IIdentityService
{
    /// <inheritdoc />
    public async Task<Guid> RegisterAsync(
        string email,
        string? username,
        string password,
        CancellationToken cancellationToken)
    {
        var registerRequest = new RegisterRequest(email, password, username);
        var registrationResult = await identityProvider.RegisterAsync(registerRequest, cancellationToken);

        await identityNotifier.SendEmailConfirmationAsync(
            registrationResult.EmailConfirmation.Email,
            registrationResult.EmailConfirmation,
            cancellationToken);

        logger.LogInformation("User {UserId} registered successfully", registrationResult.UserId);
        return registrationResult.UserId;
    }

    /// <inheritdoc />
    public Task ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("User {UserId} confirmed email", userId);
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task ResendEmailConfirmationAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        var ticket = await identityProvider.RequestEmailConfirmationAsync(identifier, cancellationToken);

        if (ticket is null)
        {
            return;
        }

        await identityNotifier.SendEmailConfirmationAsync(ticket.Email, ticket, cancellationToken);
        logger.LogInformation("Confirmation email sent for identifier {Identifier}", identifier);
    }

    /// <inheritdoc />
    public Task<LoginResult> LoginAsync(
        string identifier,
        string password,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("User {Identifier} logged in and session {SessionId} started", identifier, Guid.NewGuid());
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<TokenPair> VerifyMfaAsync(
        string challengeToken,
        string code,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("MFA verified successfully and session {SessionId} started", Guid.NewGuid());
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task ForgotPasswordAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Password reset email sent for identifier {Identifier}", identifier);
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task ResetPasswordAsync(
        Guid userId,
        string token,
        string newPassword,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Password reset for user {UserId}", userId);
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<bool> IsSessionActiveAsync(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Checked activity status for session {SessionId}", sessionId);
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task LogoutAllAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("All sessions terminated for user {UserId}", userId);
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("User logged out using refresh token");
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<TokenPair> RefreshAsync(
        string refreshToken,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Tokens refreshed successfully");
        throw new NotImplementedException();
    }
}