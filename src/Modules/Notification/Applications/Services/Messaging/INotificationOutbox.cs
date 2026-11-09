namespace NoMoreTears.Modules.Notification.Applications.Services.Messaging;

/// <summary>
///     Marker of the Notification module for <c>IEventPublisher&lt;T&gt;</c>: events are published through the transactional
///     outbox of the Notification DbContext, so they are committed together with the Notification data.
/// </summary>
public interface INotificationOutbox;