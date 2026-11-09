using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Applications.Services.Emails;
using NoMoreTears.Modules.Notification.Domain.Aggregates;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Notification.Applications.Services;

/// <summary>
///     Sends templated emails requested by events of other modules and records their delivery status.
/// </summary>
public sealed class EmailDeliveryService(
    IRepository<EmailDelivery, Guid> deliveries,
    IEmailNotifier emailNotifier,
    TimeProvider timeProvider,
    ILogger<EmailDeliveryService> logger) : IEmailDeliveryService
{
    /// <inheritdoc />
    public async Task<bool> SendAsync(
        Guid sourceEventId,
        Guid recipientId,
        string address,
        string template,
        IReadOnlyDictionary<string, string> model,
        int attempt,
        CancellationToken cancellationToken)
    {
        if (await deliveries.ExistsAsync(delivery => delivery.SourceEventId == sourceEventId, cancellationToken))
        {
            return false;
        }

        await emailNotifier.SendAsync(new EmailNotification(address, template, model), cancellationToken);

        var delivery = EmailDelivery.Sent(sourceEventId, recipientId, template, attempt, timeProvider.GetUtcNow());
        await deliveries.CreateAsync(delivery, cancellationToken);

        logger.LogInformation("Email {Template} sent to user {RecipientId} (attempt {Attempt})", template, recipientId, attempt);

        return true;
    }

    /// <inheritdoc />
    public async Task RecordFailureAsync(
        Guid sourceEventId,
        Guid recipientId,
        string template,
        int attempts,
        string error,
        CancellationToken cancellationToken)
    {
        if (await deliveries.ExistsAsync(delivery => delivery.SourceEventId == sourceEventId, cancellationToken))
        {
            return;
        }

        var delivery = EmailDelivery.Failed(sourceEventId, recipientId, template, attempts, error, timeProvider.GetUtcNow());
        await deliveries.CreateAsync(delivery, cancellationToken);

        logger.LogWarning("Email {Template} to user {RecipientId} failed after {Attempts} attempts", template, recipientId, attempts);
    }
}