using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Notification.Domain.Exceptions;

/// <summary>
///     Thrown when a notification violates its rules.
/// </summary>
public sealed class InvalidNotificationException(string message)
    : DomainException(message);