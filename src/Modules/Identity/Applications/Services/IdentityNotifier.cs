using System.Globalization;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Configuration;
using NoMoreTears.Modules.Notification.Contracts.Emails;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Modules.Identity.Applications.Services;

/// <summary>
///     Delivers identity tickets to users by email through the notification module.
/// </summary>
public sealed class IdentityNotifier(
    IEmailNotifier emailNotifier,
    IOptions<IdentityLinkOptions> options) : IIdentityNotifier
{
    private readonly IdentityLinkOptions _links = options.Value;

    /// <inheritdoc />
    public Task SendEmailConfirmationAsync(
        string email,
        UserActionTicket ticket,
        CancellationToken cancellationToken)
    {
        return SendAsync(
            email,
            ticket,
            UserActionPurpose.EmailConfirmation,
            EmailTemplates.EmailConfirmation,
            _links.EmailConfirmationUrl,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task SendPasswordResetAsync(
        string email,
        UserActionTicket ticket,
        CancellationToken cancellationToken)
    {
        return SendAsync(
            email,
            ticket,
            UserActionPurpose.PasswordReset,
            EmailTemplates.PasswordReset,
            _links.PasswordResetUrl,
            cancellationToken);
    }

    private Task SendAsync(
        string email,
        UserActionTicket ticket,
        UserActionPurpose purpose,
        string template,
        string urlTemplate,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(ticket);

        if (ticket.Purpose != purpose)
        {
            throw new ArgumentException($"Ticket purpose must be {purpose}.", nameof(ticket));
        }

        var model = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Email"] = email,
            ["Link"] = IdentityLinks.Build(urlTemplate, ticket),
            ["ExpiresAt"] = ticket.ExpiresAt.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture)
        };

        return emailNotifier.SendAsync(new EmailNotification(email, template, model), cancellationToken);
    }
}