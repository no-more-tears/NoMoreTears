using System.Net.Sockets;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Applications.Services.Configuration;
using NoMoreTears.Modules.Notification.Applications.Services.Templates;
using NoMoreTears.Modules.Notification.Contracts.Emails;
using NoMoreTears.Shared.Common.Exceptions.Integration;

namespace NoMoreTears.Modules.Notification.Applications.Services.Smtp;

/// <summary>
///     Renders templated notifications and delivers them through the configured SMTP server.
/// </summary>
/// <remarks>
///     A connection is opened per email. SMTP failures surface as <see cref="ExternalAuthException"/>,
///     <see cref="ExternalTimeoutException"/> or <see cref="ExternalUnavailableException"/>.
/// </remarks>
public sealed class SmtpEmailNotifier(
    IEmailTemplateStore templateStore,
    IOptions<SmtpOptions> options,
    ILogger<SmtpEmailNotifier> logger) : IEmailNotifier
{
    private readonly SmtpOptions _smtp = options.Value;

    public async Task SendAsync(
        EmailNotification notification,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var template = await templateStore.GetAsync(notification.Template, cancellationToken);
        var content = EmailTemplateRenderer.Render(template, notification.Model);

        using var message = CreateMessage(notification.To, content);

        try
        {
            await DeliverAsync(message, cancellationToken);
        }
        catch (AuthenticationException exception)
        {
            logger.LogError(exception, "Email {Template} could not be delivered over SMTP.", notification.Template);
            throw new ExternalAuthException("SMTP server rejected the credentials.");
        }
        catch (TimeoutException exception)
        {
            logger.LogError(exception, "Email {Template} could not be delivered over SMTP.", notification.Template);
            throw new ExternalTimeoutException("SMTP server did not respond in time.");
        }
        catch (Exception exception) when (exception is CommandException
            or ProtocolException
            or SslHandshakeException
            or SocketException
            or IOException)
        {
            logger.LogError(exception, "Email {Template} could not be delivered over SMTP.", notification.Template);
            throw new ExternalUnavailableException("SMTP server is unavailable or rejected the email.");
        }

        logger.LogInformation("Email {Template} sent. MessageId={MessageId}", notification.Template, message.MessageId);
    }

    private MimeMessage CreateMessage(
        string to,
        RenderedEmail content)
    {
        var body = new BodyBuilder
        {
            HtmlBody = content.HtmlBody,
            TextBody = content.TextBody
        };

        var message = new MimeMessage
        {
            Subject = content.Subject,
            Body = body.ToMessageBody()
        };

        message.From.Add(new MailboxAddress(_smtp.SenderName, _smtp.SenderAddress));
        message.To.Add(MailboxAddress.Parse(to));

        return message;
    }

    private async Task DeliverAsync(
        MimeMessage message,
        CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();
        client.Timeout = (int)_smtp.Timeout.TotalMilliseconds;

        await client.ConnectAsync(_smtp.Host, _smtp.Port, _smtp.Security, cancellationToken);

        if (!string.IsNullOrEmpty(_smtp.Username))
        {
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password ?? string.Empty, cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}
