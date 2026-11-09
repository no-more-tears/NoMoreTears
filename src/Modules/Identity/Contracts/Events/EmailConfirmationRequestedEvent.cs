using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Identity.Contracts.Events;

/// <summary>
///     The user has to confirm the email address: after registration or on request.
///     Notification sends the confirmation email with <see cref="ConfirmationUrl"/>.
/// </summary>
/// <remarks>
///     The link carries a single-use token, so the event is sensitive: it lives in the outbox only until delivery.
/// </remarks>
public sealed record EmailConfirmationRequestedEvent(
    Guid UserId,
    string Email,
    string ConfirmationUrl,
    DateTimeOffset ExpiresAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
