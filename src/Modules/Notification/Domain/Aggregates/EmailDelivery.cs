using NoMoreTears.Modules.Notification.Domain.Enums;
using NoMoreTears.Modules.Notification.Domain.Exceptions;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Notification.Domain.Aggregates;

/// <summary>
///     Delivery status of one email. The address and the content are not stored:
///     emails may carry single-use links and codes.
/// </summary>
public sealed class EmailDelivery : AuditableEntity<Guid>
{
    /// <summary>
    ///     Event of another module that asked for the email; one event leads to at most one delivery.
    /// </summary>
    public Guid SourceEventId { get; private set; }

    /// <summary>
    ///     User the email is addressed to.
    /// </summary>
    public Guid RecipientId { get; private set; }

    /// <summary>
    ///     Name of the email template.
    /// </summary>
    public string Template { get; private set; }

    /// <summary>
    ///     Final status.
    /// </summary>
    public DeliveryStatus Status { get; private set; }

    /// <summary>
    ///     Number of attempts, including retries with exponential delay.
    /// </summary>
    public int Attempts { get; private set; }

    /// <summary>
    ///     Error of the last failed attempt.
    /// </summary>
    public string? LastError { get; private set; }

    /// <summary>
    ///     Moment the status was set.
    /// </summary>
    public DateTimeOffset CompletedAt { get; private set; }

    /// <summary>
    ///     Used only by persistence to materialize the delivery.
    /// </summary>
    private EmailDelivery()
    {
        Template = null!;
    }

    private EmailDelivery(
        Guid id,
        Guid sourceEventId,
        Guid recipientId,
        string template,
        DeliveryStatus status,
        int attempts,
        string? lastError,
        DateTimeOffset completedAt)
    {
        Id = id;
        SourceEventId = sourceEventId;
        RecipientId = recipientId;
        Template = template;
        Status = status;
        Attempts = attempts;
        LastError = lastError;
        CompletedAt = completedAt;
    }

    /// <summary>
    ///     Records an email that the gateway accepted.
    /// </summary>
    public static EmailDelivery Sent(
        Guid sourceEventId,
        Guid recipientId,
        string template,
        int attempts,
        DateTimeOffset sentAt) => 
        Create(sourceEventId, recipientId, template, DeliveryStatus.Sent, attempts, null, sentAt);

    /// <summary>
    ///     Records an email that could not be delivered after every retry.
    /// </summary>
    public static EmailDelivery Failed(
        Guid sourceEventId,
        Guid recipientId,
        string template,
        int attempts,
        string error,
        DateTimeOffset failedAt)
    {
        var normalizedError = error?.Trim() ?? string.Empty;

        if (normalizedError.Length == 0)
        {
            throw new InvalidEmailDeliveryException("A failed delivery must have an error.");
        }

        if (normalizedError.Length > NotificationRules.MaxErrorLength)
        {
            normalizedError = normalizedError[..NotificationRules.MaxErrorLength];
        }

        return Create(sourceEventId, recipientId, template, DeliveryStatus.Failed, attempts, normalizedError, failedAt);
    }

    private static EmailDelivery Create(
        Guid sourceEventId,
        Guid recipientId,
        string template,
        DeliveryStatus status,
        int attempts,
        string? error,
        DateTimeOffset completedAt)
    {
        if (sourceEventId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(sourceEventId));
        }

        if (recipientId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(recipientId));
        }

        var normalizedTemplate = template?.Trim() ?? string.Empty;

        if (normalizedTemplate.Length == 0 || normalizedTemplate.Length > NotificationRules.MaxTemplateLength)
        {
            throw new InvalidEmailDeliveryException(
                $"Template must contain from 1 to {NotificationRules.MaxTemplateLength} characters.");
        }

        if (attempts < 1)
        {
            throw new InvalidEmailDeliveryException("A delivery must have at least one attempt.");
        }

        var delivery = new EmailDelivery(
            Guid.CreateVersion7(completedAt),
            sourceEventId,
            recipientId,
            normalizedTemplate,
            status,
            attempts,
            error,
            completedAt);

        delivery.MarkAsCreated(completedAt);
        return delivery;
    }
}