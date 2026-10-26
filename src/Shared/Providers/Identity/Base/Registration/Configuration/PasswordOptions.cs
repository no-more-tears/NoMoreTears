using System.ComponentModel.DataAnnotations;

namespace NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

/// <summary>
///     Password hashing and policy settings.
/// </summary>
public sealed class PasswordOptions
{
    /// <summary>
    ///     Configuration section name, relative to the identity section.
    /// </summary>
    public const string SectionName = "Passwords";

    /// <summary>
    ///     BCrypt cost factor. Hashes produced with a lower factor are transparently
    ///     recomputed on the next successful sign-in.
    /// </summary>
    [Range(10, 16)]
    public int WorkFactor { get; set; } = 12;

    /// <summary>
    ///     Minimum accepted password length.
    /// </summary>
    [Range(6, 256)]
    public int MinimumLength { get; set; } = 8;

    /// <summary>
    ///     Maximum accepted password length.
    /// </summary>
    [Range(8, 1024)]
    public int MaximumLength { get; set; } = 128;
}