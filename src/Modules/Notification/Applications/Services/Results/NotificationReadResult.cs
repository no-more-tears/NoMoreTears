using NoMoreTears.Modules.Notification.Contracts.Responses;

namespace NoMoreTears.Modules.Notification.Applications.Services.Results;

/// <summary>
///     A notification after it was read, with the change to broadcast to the other clients of the recipient.
/// </summary>
public sealed record NotificationReadResult(
    NotificationResponse Notification,
    NotificationsChangedResponse Change);