using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Notification.Domain.Exceptions;

/// <summary>
///     Thrown when an email delivery record violates its rules.
/// </summary>
public sealed class InvalidEmailDeliveryException(string message)
    : DomainException(message);