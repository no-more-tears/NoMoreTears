using MailKit.Security;

namespace NoMoreTears.Modules.Notification.Applications.Services.Configuration;

/// <summary>
///     SMTP server and sender used to deliver emails.
/// </summary>
public sealed class SmtpOptions
{
    /// <summary>
    ///     Configuration sub-section of the notification module.
    /// </summary>
    public const string SectionName = "Smtp";

    /// <summary>
    ///     SMTP server host name.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    ///     SMTP server port.
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    ///     Transport security: <c>StartTls</c>, <c>SslOnConnect</c>, <c>StartTlsWhenAvailable</c>, <c>Auto</c> or <c>None</c>.
    /// </summary>
    public SecureSocketOptions Security { get; set; } = SecureSocketOptions.StartTls;

    /// <summary>
    ///     Login of the SMTP account; authentication is skipped when empty.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     Password of the SMTP account. Keep it in user secrets or environment variables.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///     Address emails are sent from.
    /// </summary>
    public string SenderAddress { get; set; } = string.Empty;

    /// <summary>
    ///     Display name of the sender.
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    ///     Timeout of a single SMTP operation.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
