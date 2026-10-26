using System.ComponentModel.DataAnnotations;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

/// <summary>
///     Settings of sessions and the refresh tokens that keep them alive.
/// </summary>
public sealed class SessionOptions
{
    /// <summary>
    ///     Configuration section name, relative to the identity section.
    /// </summary>
    public const string SectionName = "Sessions";

    /// <summary>
    ///     Sliding lifetime of a refresh token; every rotation issues a token with this lifetime.
    /// </summary>
    [Range(typeof(TimeSpan), "00:05:00", "365.00:00:00")]
    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    ///     Absolute lifetime of a session; no rotation can extend a session beyond it.
    /// </summary>
    [Range(typeof(TimeSpan), "00:05:00", "365.00:00:00")]
    public TimeSpan SessionLifetime { get; set; } = TimeSpan.FromDays(30);

    /// <summary>
    ///     How long expired and revoked sessions are kept for auditing before maintenance removes them.
    /// </summary>
    [Range(typeof(TimeSpan), "00:00:00", "3650.00:00:00")]
    public TimeSpan RevokedSessionRetention { get; set; } = TimeSpan.FromDays(30);
}