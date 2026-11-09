using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Identity.Contracts.Events;

/// <summary>
///     A sign-in requires a one-time code. Notification sends <see cref="Code"/> by email.
/// </summary>
/// <remarks>
///     The code completes a sign-in, so the event is sensitive: it lives in the outbox only until delivery.
/// </remarks>
public sealed record MfaCodeIssuedEvent(
    Guid UserId,
    string Email,
    string Code,
    DateTimeOffset ExpiresAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
