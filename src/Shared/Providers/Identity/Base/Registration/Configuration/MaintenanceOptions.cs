using System.ComponentModel.DataAnnotations;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

/// <summary>
///     Settings of the optional background cleanup of identity storage.
/// </summary>
public sealed class MaintenanceOptions
{
    /// <summary>
    ///     Period between cleanup runs.
    /// </summary>
    [Range(typeof(TimeSpan), "00:01:00", "7.00:00:00")]
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
}