using System.ComponentModel.DataAnnotations;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

/// <summary>
///     Settings of the JWT access tokens issued and validated by the provider.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    ///     Configuration section name, relative to the identity section.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    ///     Minimum length of <see cref="SigningKey"/> in characters, which yields 256 bits of key material.
    /// </summary>
    public const int MinimumSigningKeyLength = 32;

    /// <summary>
    ///     Value of the <c>iss</c> claim.
    /// </summary>
    [Required]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    ///     Value of the <c>aud</c> claim.
    /// </summary>
    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    ///     Symmetric key used with HMAC-SHA256. The UTF-8 bytes of the string are the key material,
    ///     so the value should be a long random string rather than a human-chosen phrase.
    /// </summary>
    [Required]
    [MinLength(MinimumSigningKeyLength)]
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>
    ///     Lifetime of an access token. Keep it short, revocation is enforced through the session.
    /// </summary>
    [Range(typeof(TimeSpan), "00:01:00", "1.00:00:00")]
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    ///     Tolerance applied when validating token lifetime.
    /// </summary>
    [Range(typeof(TimeSpan), "00:00:00", "00:05:00")]
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(30);
}
