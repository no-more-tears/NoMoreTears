namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.RecordEmailFailure;

/// <summary>
///     Records an email that could not be delivered after every retry.
/// </summary>
public sealed record RecordEmailFailureCommand(
    Guid SourceEventId,
    Guid RecipientId,
    string Template,
    int Attempts,
    string Error) : ICommand;
