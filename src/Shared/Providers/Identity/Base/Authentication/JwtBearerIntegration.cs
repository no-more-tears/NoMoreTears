namespace NoMoreTears.Shared.Providers.Identity.Authentication;

/// <summary>
///     A bearer authentication scheme that validates tokens issued by the identity provider.
/// </summary>
/// <param name="Scheme">Authentication scheme name.</param>
/// <param name="ValidateSessions">Whether tokens of revoked sessions are rejected.</param>
internal sealed record JwtBearerIntegration(
    string Scheme,
    bool ValidateSessions);