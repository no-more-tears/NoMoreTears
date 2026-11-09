namespace NoMoreTears.Modules.Notification.Applications.Services.Abstractions;

/// <summary>
///     Sends templated emails requested by events of other modules and records their delivery status.
/// </summary>
public interface IEmailDeliveryService
{
    /// <summary>
    ///     Sends the email and records it as sent; returns <c>false</c> when the event was already delivered.
    ///     A failure is thrown, so the message is retried with exponential delay.
    /// </summary>
    Task<bool> SendAsync(
        Guid sourceEventId,
        Guid recipientId,
        string address,
        string template,
        IReadOnlyDictionary<string, string> model,
        int attempt,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Records an email that could not be delivered after every retry.
    /// </summary>
    Task RecordFailureAsync(
        Guid sourceEventId,
        Guid recipientId,
        string template,
        int attempts,
        string error,
        CancellationToken cancellationToken);
}