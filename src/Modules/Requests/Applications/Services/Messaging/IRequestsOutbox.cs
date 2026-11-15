namespace NoMoreTears.Modules.Requests.Applications.Services.Messaging;

/// <summary>
///     Marker of the Requests module for <c>IEventPublisher&lt;T&gt;</c>: events are published through the transactional
///     outbox of the Requests DbContext, so they are committed together with the Requests data.
/// </summary>
public interface IRequestsOutbox;