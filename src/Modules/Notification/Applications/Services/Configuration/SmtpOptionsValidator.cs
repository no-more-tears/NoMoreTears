using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace NoMoreTears.Modules.Notification.Applications.Services.Configuration;

/// <summary>
///     Validates <see cref="SmtpOptions"/> on application start.
/// </summary>
public sealed class SmtpOptionsValidator : IValidateOptions<SmtpOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        SmtpOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var builder = new ValidateOptionsResultBuilder();

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            builder.AddError("SMTP host is required.", nameof(SmtpOptions.Host));
        }

        if (options.Port is < 1 or > 65535)
        {
            builder.AddError("SMTP port must be between 1 and 65535.", nameof(SmtpOptions.Port));
        }

        if (!MailAddress.TryCreate(options.SenderAddress, out _))
        {
            builder.AddError("Sender address must be a valid email address.", nameof(SmtpOptions.SenderAddress));
        }

        if (!string.IsNullOrEmpty(options.Username) && string.IsNullOrEmpty(options.Password))
        {
            builder.AddError("SMTP password is required when a username is set.", nameof(SmtpOptions.Password));
        }

        if (options.Timeout <= TimeSpan.Zero)
        {
            builder.AddError("SMTP timeout must be positive.", nameof(SmtpOptions.Timeout));
        }

        return builder.Build();
    }
}
