using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using NoMoreTears.Shared.Providers.Identity.Persistence.Entities;
using System.Globalization;
using System.Security.Claims;

namespace NoMoreTears.Shared.Providers.Identity.Security;

/// <summary>
///     Builds the claim set embedded into access tokens.
/// </summary>
internal static class ClaimsBuilder
{
    /// <summary>
    ///     Creates subject, email, name, session, authentication method and role claims.
    /// </summary>
    public static List<Claim> Build(
        UserEntity user,
        SessionEntity session)
    {
        var claims = new List<Claim>(6 + user.Roles.Count)
        {
            new(IdentityClaimTypes.Subject, Format(user.Id)),
            new(IdentityClaimTypes.Email, user.Email),
            new(IdentityClaimTypes.Name, user.Username),
            new(IdentityClaimTypes.SessionId, Format(session.Id)),
            new(IdentityClaimTypes.AuthenticationMethods, AuthenticationMethods.Password)
        };

        if (session.MfaVerified)
        {
            claims.Add(new Claim(IdentityClaimTypes.AuthenticationMethods, AuthenticationMethods.OneTimeCode));
        }

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(IdentityClaimTypes.Role, role.Role.ToString()));
        }

        return claims;
    }

    private static string Format(Guid value) =>
        value.ToString("D", CultureInfo.InvariantCulture);
}