using System.ComponentModel.DataAnnotations;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

/// <summary>
///     Settings of single-use tokens issued for email confirmation and password reset.
/// </summary>
public sealed class UserActionTokenOptions
{
    /// <summary>
    ///     Configuration section name, relative to the identity section.
    /// </summary>
    public const string SectionName = "UserActionTokens";

    /// <summary>
    ///     Period during which an email confirmation token can be consumed.
    /// </summary>
    [Range(typeof(TimeSpan), "00:05:00", "30.00:00:00")]
    public TimeSpan EmailConfirmationLifetime { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    ///     Period during which a password reset token can be consumed.
    /// </summary>
    [Range(typeof(TimeSpan), "00:05:00", "1.00:00:00")]
    public TimeSpan PasswordResetLifetime { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    ///     Minimum period between two requested tokens of the same purpose for one user.
    ///     Zero disables the cooldown.
    /// </summary>
    [Range(typeof(TimeSpan), "00:00:00", "01:00:00")]
    public TimeSpan ReissueCooldown { get; set; } = TimeSpan.FromMinutes(1);
}