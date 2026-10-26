namespace NoMoreTears.Modules.Notification.Contracts.Emails;

/// <summary>
///     Sends templated email notifications on behalf of other modules.
/// </summary>
public interface IEmailNotifier
{
    /// <summary>
    ///     Renders the template of the notification with its model and delivers the result.
    /// </summary>
    Task SendAsync(
        EmailNotification notification,
        CancellationToken cancellationToken);
}