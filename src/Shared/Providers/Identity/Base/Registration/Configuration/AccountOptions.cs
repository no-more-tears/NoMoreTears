using System.ComponentModel.DataAnnotations;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

/// <summary>
///     Account policy settings.
/// </summary>
public sealed class AccountOptions
{
    /// <summary>
    ///     Configuration section name, relative to the identity section.
    /// </summary>
    public const string SectionName = "Account";

    /// <summary>
    ///     Blocks sign-in until the email address has been confirmed.
    /// </summary>
    public bool RequireConfirmedEmail { get; set; } = true;

    /// <summary>
    ///     Number of consecutive failed sign-in attempts that locks the account.
    ///     Zero disables the lockout.
    /// </summary>
    [Range(0, 100)]
    public int MaxFailedAccessAttempts { get; set; } = 5;

    /// <summary>
    ///     Period for which a locked account rejects sign-in.
    /// </summary>
    [Range(typeof(TimeSpan), "00:00:30", "1.00:00:00")]
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
}
