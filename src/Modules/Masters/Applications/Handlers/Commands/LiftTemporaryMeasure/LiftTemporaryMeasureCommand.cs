namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftTemporaryMeasure;

/// <summary>
///     Removes the Masters effects of an ended temporary measure of Trust. Sent by the Masters consumers.
/// </summary>
public sealed record LiftTemporaryMeasureCommand(
    Guid MasterId,
    Guid MeasureId,
    bool SuspendsFeed,
    bool HoldsVerification,
    DateTimeOffset LiftedAt) : ICommand;
