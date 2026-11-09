namespace NoMoreTears.Modules.Masters.Applications.Services.Messaging;

/// <summary>
///     Marker of the Masters module for <c>IEventPublisher&lt;T&gt;</c>: events are published through the transactional
///     outbox of the Masters DbContext, so they are committed together with the Masters data.
/// </summary>
public interface IMastersOutbox;