using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Identity.Contracts.Events;

/// <summary>
///     The user asked to reset the password. Notification sends the email with <see cref="ResetUrl"/>.
/// </summary>
/// <remarks>
///     The link carries a single-use token, so the event is sensitive: it lives in the outbox only until delivery.
/// </remarks>
public sealed record PasswordResetRequestedEvent(
    Guid UserId,
    string Email,
    string ResetUrl,
    DateTimeOffset ExpiresAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
