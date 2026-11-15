namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.LiftClientRestriction;

/// <summary>
///     Lifts a publication restriction of a client. Sent by the Requests consumers.
/// </summary>
public sealed record LiftClientRestrictionCommand(
    Guid RestrictionId,
    DateTimeOffset LiftedAt) : ICommand;