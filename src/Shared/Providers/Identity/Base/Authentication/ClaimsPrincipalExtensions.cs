using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using System.Security.Claims;

namespace NoMoreTears.Shared.Providers.Identity.Authentication;

/// <summary>
///     Reads identity claims from a principal authenticated with a token issued by the provider.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <param name="principal">Authenticated principal.</param>
    extension(ClaimsPrincipal principal)
    {
        /// <summary>
        ///     Returns the user identifier from the <c>sub</c> claim.
        /// </summary>
        public Guid? GetUserId() => GetGuid(principal, IdentityClaimTypes.Subject);

        /// <summary>
        ///     Returns the session identifier from the <c>sid</c> claim.
        /// </summary>
        public Guid? GetSessionId() => GetGuid(principal, IdentityClaimTypes.SessionId);

        /// <summary>
        ///     Returns the roles from the <c>role</c> claims; unknown values are skipped.
        /// </summary>
        public IReadOnlyCollection<UserRole> GetRoles()
        {
            ArgumentNullException.ThrowIfNull(principal);

            var roles = new List<UserRole>();

            foreach (var claim in principal.FindAll(IdentityClaimTypes.Role))
            {
                if (Enum.TryParse<UserRole>(claim.Value, ignoreCase: false, out var role))
                {
                    roles.Add(role);
                }
            }

            return roles;
        }

        /// <summary>
        ///     Returns whether the session behind the token was verified with a second factor.
        /// </summary>
        public bool HasVerifiedMfa()
        {
            ArgumentNullException.ThrowIfNull(principal);
            return principal.HasClaim(IdentityClaimTypes.AuthenticationMethods, AuthenticationMethods.OneTimeCode);
        }
    }

    private static Guid? GetGuid(
        ClaimsPrincipal principal,
        string claimType)
    {
        ArgumentNullException.ThrowIfNull(principal);

        return Guid.TryParse(principal.FindFirst(claimType)?.Value, out var value)
            ? value
            : null;
    }
}