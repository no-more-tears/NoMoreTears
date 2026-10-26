namespace NoMoreTears.Modules.Identity.Applications.Services.Configuration;

/// <summary>
///     Client URLs included in identity emails.
/// </summary>
public sealed class IdentityLinkOptions
{
    /// <summary>
    ///     Configuration sub-section of the identity module.
    /// </summary>
    public const string SectionName = "Links";

    /// <summary>
    ///     Page of the client that confirms the email address.
    /// </summary>
    public string EmailConfirmationUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Page of the client that sets a new password.
    /// </summary>
    public string PasswordResetUrl { get; set; } = string.Empty;
}