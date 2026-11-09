namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftSanction;

/// <summary>
///     Lifts a sanction of Trust from a master. Sent by the Masters consumers.
/// </summary>
public sealed record LiftSanctionCommand(
    Guid MasterId,
    Guid SanctionId,
    DateTimeOffset LiftedAt) : ICommand;
