namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;

/// <summary>
///     Stores an in-app notification caused by an event of another module. Sent by the consumers of the SignalR host.
/// </summary>
public sealed record CreateNotificationCommand(
    Guid RecipientId,
    Guid SourceEventId,
    string Type,
    IReadOnlyDictionary<string, string> Data,
    DateTimeOffset OccurredAt) : ICommand;
