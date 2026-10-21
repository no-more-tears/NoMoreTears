using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Services;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;
using NoMoreTears.Shared.Providers.Identity.Security;

namespace NoMoreTears.Shared.Providers.Identity.Authentication;

/// <summary>
///     Configures bearer schemes registered through the builder: validation parameters derived
///     from <see cref="JwtOptions"/>, stable claim names and rejection of revoked sessions.
/// </summary>
internal sealed class JwtBearerOptionsConfigurator(
    IOptions<JwtOptions> jwtOptions,
    IEnumerable<JwtBearerIntegration> integrations) : IConfigureNamedOptions<JwtBearerOptions>
{
    /// <inheritdoc />
    public void Configure(
        string? name,
        JwtBearerOptions options)
    {
        var integration = integrations.FirstOrDefault(x => string.Equals(x.Scheme, name, StringComparison.Ordinal));

        if (integration is null)
        {
            return;
        }

        options.MapInboundClaims = false;
        options.TokenValidationParameters = JwtProvider.CreateValidationParameters(jwtOptions.Value);

        if (!integration.ValidateSessions)
        {
            return;
        }

        options.Events ??= new JwtBearerEvents();
        var next = options.Events.OnTokenValidated;

        options.Events.OnTokenValidated = async context =>
        {
            await next(context);

            if (context.Result is null)
            {
                await RejectRevokedSessionAsync(context);
            }
        };
    }

    /// <inheritdoc />
    public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);

    private static async Task RejectRevokedSessionAsync(TokenValidatedContext context)
    {
        var sessionIdValue = context.Principal?.FindFirst(IdentityClaimTypes.SessionId)?.Value;

        if (!Guid.TryParse(sessionIdValue, out var sessionId))
        {
            context.Fail("The token does not carry a session identifier.");
            return;
        }

        var sessions = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();

        if (!await sessions.IsSessionActiveAsync(sessionId, context.HttpContext.RequestAborted))
        {
            context.Fail("The session has been revoked.");
        }
    }
}