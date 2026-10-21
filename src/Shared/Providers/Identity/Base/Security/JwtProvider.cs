using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Security;
using NoMoreTears.Shared.Providers.Identity.Contracts.Tokens;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace NoMoreTears.Shared.Providers.Identity.Security;

/// <summary>
///     Issues HMAC-SHA256 signed access tokens of type <c>at+jwt</c> (RFC 9068)
///     and creates the matching validation parameters.
/// </summary>
internal sealed class JwtProvider : IJwtProvider
{
    /// <summary>
    ///     Value of the <c>typ</c> header of issued tokens.
    /// </summary>
    public const string AccessTokenType = "at+jwt";

    private static readonly string[] ValidAlgorithms = [SecurityAlgorithms.HmacSha256];
    private static readonly string[] ValidTypes = [AccessTokenType];

    private static readonly JsonWebTokenHandler Handler = new()
    {
        SetDefaultTimesOnTokenCreation = false
    };

    private readonly JwtOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly SigningCredentials _credentials;

    public JwtProvider(
        IOptions<JwtOptions> options,
        TimeProvider timeProvider)
    {
        _options = options.Value;
        _timeProvider = timeProvider;
        _credentials = new SigningCredentials(CreateKey(_options), SecurityAlgorithms.HmacSha256);
    }

    /// <inheritdoc />
    public AccessToken CreateAccessToken(IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        var now = _timeProvider.GetUtcNow();
        var expiresAt = now + _options.AccessTokenLifetime;
        var tokenId = Guid.CreateVersion7().ToString("N", CultureInfo.InvariantCulture);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            TokenType = AccessTokenType,
            SigningCredentials = _credentials,
            Subject = new ClaimsIdentity([.. claims, new Claim(IdentityClaimTypes.TokenId, tokenId)])
        };

        return new AccessToken(Handler.CreateToken(descriptor), tokenId, expiresAt);
    }

    /// <summary>
    ///     Creates validation parameters that accept only tokens issued with the same options.
    /// </summary>
    public static TokenValidationParameters CreateValidationParameters(JwtOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer,
            ValidateAudience = true,
            ValidAudience = options.Audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = CreateKey(options),
            ValidAlgorithms = ValidAlgorithms,
            ValidTypes = ValidTypes,
            ClockSkew = options.ClockSkew,
            NameClaimType = IdentityClaimTypes.Name,
            RoleClaimType = IdentityClaimTypes.Role
        };
    }

    private static SymmetricSecurityKey CreateKey(JwtOptions options) =>
        new(Encoding.UTF8.GetBytes(options.SigningKey));
}