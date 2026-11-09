namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.SendEmail;

/// <summary>
///     Sends a templated email requested by an event of another module and records its delivery.
/// </summary>
public sealed record SendEmailCommand(
    Guid SourceEventId,
    Guid RecipientId,
    string Address,
    string Template,
    IReadOnlyDictionary<string, string> Model,
    int Attempt) : ICommand;
