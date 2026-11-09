namespace NoMoreTears.Modules.Identity.Applications.Services.Messaging;

/// <summary>
///     Marker of the Identity module for <c>IEventPublisher&lt;T&gt;</c>: events are published through the transactional
///     outbox of the Identity DbContext, so they are committed together with the Identity data.
/// </summary>
public interface IIdentityOutbox;
