using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using System.Security.Claims;

namespace NoMoreTears.Shared.Providers.Identity.Abstractions.Security;

/// <summary>
///     Issues signed JWT access tokens.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    ///     Creates a signed access token carrying the claims plus a unique <c>jti</c>.
    /// </summary>
    AccessToken CreateAccessToken(IEnumerable<Claim> claims);
}