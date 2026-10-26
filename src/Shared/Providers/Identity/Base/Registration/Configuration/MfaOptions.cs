using System.ComponentModel.DataAnnotations;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

/// <summary>
///     Settings of one-time multi-factor authentication challenges.
/// </summary>
public sealed class MfaOptions
{
    /// <summary>
    ///     Configuration section name, relative to the identity section.
    /// </summary>
    public const string SectionName = "Mfa";

    /// <summary>
    ///     Number of digits in the verification code.
    /// </summary>
    [Range(4, 10)]
    public int CodeLength { get; set; } = 6;

    /// <summary>
    ///     Period during which a challenge can be verified.
    /// </summary>
    [Range(typeof(TimeSpan), "00:00:30", "01:00:00")]
    public TimeSpan ChallengeLifetime { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     Number of wrong codes after which the challenge is rejected.
    /// </summary>
    [Range(1, 10)]
    public int MaxAttempts { get; set; } = 5;
}