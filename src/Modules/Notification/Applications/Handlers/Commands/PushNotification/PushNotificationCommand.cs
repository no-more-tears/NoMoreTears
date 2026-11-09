namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotification;

/// <summary>
///     Pushes a stored notification to the connected clients of its recipient.
/// </summary>
public sealed record PushNotificationCommand(
    Guid NotificationId) : ICommand;
