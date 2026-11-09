using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Identity.Applications.Services.Configuration;
using NoMoreTears.Modules.Identity.Applications.Services.Mappings;
using NoMoreTears.Modules.Identity.Applications.Services.Results;
using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.Providers.Identity.Abstractions;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;
using NoMoreTears.Shared.Providers.Identity.Contracts.Passwords;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignUp;

namespace NoMoreTears.Modules.Identity.Applications.Services;

/// <summary>
///     Provides identity management functionality.
/// </summary>
public sealed class IdentityService(
    IIdentityProvider identityProvider,
    IOptions<IdentityLinkOptions> linkOptions,
    TimeProvider timeProvider,
    ILogger<IdentityService> logger) : IIdentityService
{
    private readonly IdentityLinkOptions _links = linkOptions.Value;

    /// <inheritdoc />
    public async Task<RegistrationOutcome> RegisterAsync(
        string email,
        string? username,
        string password,
        CancellationToken cancellationToken)
    {
        var registerRequest = new RegisterRequest(email, password, username);
        var registrationResult = await identityProvider.RegisterAsync(registerRequest, cancellationToken);

        var emailConfirmation = registrationResult.EmailConfirmation.ToEmailConfirmationRequestedEvent(
            _links.EmailConfirmationUrl,
            timeProvider.GetUtcNow());

        logger.LogInformation("User {UserId} registered successfully", registrationResult.UserId);
        return new RegistrationOutcome(registrationResult.UserId, emailConfirmation);
    }

    /// <inheritdoc />
    public async Task ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken)
    {
        var confirmEmailRequest = new ConfirmEmailRequest(userId, token);
        await identityProvider.ConfirmEmailAsync(confirmEmailRequest, cancellationToken);

        logger.LogInformation("User {UserId} confirmed email", userId);
    }

    /// <inheritdoc />
    public async Task<EmailConfirmationRequestedEvent?> ResendEmailConfirmationAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        var ticket = await identityProvider.RequestEmailConfirmationAsync(identifier, cancellationToken);

        // Unknown and already confirmed accounts are not reported to the caller.
        if (ticket is null)
        {
            return null;
        }

        logger.LogInformation("Email confirmation requested again for user {UserId}", ticket.UserId);
        return ticket.ToEmailConfirmationRequestedEvent(_links.EmailConfirmationUrl, timeProvider.GetUtcNow());
    }

    /// <inheritdoc />
    public async Task<LoginOutcome> LoginAsync(
        string identifier,
        string password,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        var loginRequest = new LoginRequest(identifier, password, client);
        var loginResult = await identityProvider.LoginAsync(loginRequest, cancellationToken);

        if (loginResult.IsMfaRequired)
        {
            logger.LogInformation("MFA challenge issued for user {UserId}", loginResult.MfaChallenge.UserId);

            return new LoginOutcome(
                loginResult.ToResponse(),
                loginResult.MfaChallenge.ToMfaCodeIssuedEvent(timeProvider.GetUtcNow()));
        }

        logger.LogInformation("User logged in and session {SessionId} started", loginResult.Tokens.SessionId);
        return new LoginOutcome(loginResult.ToResponse(), null);
    }

    /// <inheritdoc />
    public async Task<TokenResponse> VerifyMfaAsync(
        string challengeToken,
        string code,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        var verifyMfaRequest = new VerifyMfaRequest(challengeToken, code, client);
        var tokens = await identityProvider.VerifyMfaAsync(verifyMfaRequest, cancellationToken);

        logger.LogInformation("MFA verified and session {SessionId} started", tokens.SessionId);
        return tokens.ToResponse();
    }

    /// <inheritdoc />
    public async Task<TokenResponse> RefreshAsync(
        string refreshToken,
        ClientContext client,
        CancellationToken cancellationToken)
    {
        var refreshRequest = new RefreshRequest(refreshToken, client);
        var tokens = await identityProvider.RefreshAsync(refreshRequest, cancellationToken);

        logger.LogInformation("Tokens refreshed for session {SessionId}", tokens.SessionId);
        return tokens.ToResponse();
    }

    /// <inheritdoc />
    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        await identityProvider.LogoutAsync(refreshToken, cancellationToken);

        logger.LogInformation("User logged out using refresh token");
    }

    /// <inheritdoc />
    public async Task LogoutAllAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        await identityProvider.LogoutAllAsync(userId, cancellationToken);

        logger.LogInformation("All sessions terminated for user {UserId}", userId);
    }

    /// <inheritdoc />
    public async Task<PasswordResetRequestedEvent?> ForgotPasswordAsync(
        string identifier,
        CancellationToken cancellationToken)
    {
        var ticket = await identityProvider.ForgotPasswordAsync(identifier, cancellationToken);

        // Unknown accounts are not reported to the caller.
        if (ticket is null)
        {
            return null;
        }

        logger.LogInformation("Password reset requested for user {UserId}", ticket.UserId);
        return ticket.ToPasswordResetRequestedEvent(_links.PasswordResetUrl, timeProvider.GetUtcNow());
    }

    /// <inheritdoc />
    public async Task ResetPasswordAsync(
        Guid userId,
        string token,
        string newPassword,
        CancellationToken cancellationToken)
    {
        var resetPasswordRequest = new ResetPasswordRequest(userId, token, newPassword);
        await identityProvider.ResetPasswordAsync(resetPasswordRequest, cancellationToken);

        logger.LogInformation("Password reset for user {UserId}", userId);
    }
}