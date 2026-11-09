namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplyTemporaryMeasure;

/// <summary>
///     Applies the Masters effects of a temporary measure of Trust (feed suspension, verification hold). Sent by the Masters consumers.
/// </summary>
public sealed record ApplyTemporaryMeasureCommand(
    Guid MasterId,
    Guid MeasureId,
    string Rule,
    bool SuspendsFeed,
    bool HoldsVerification,
    DateTimeOffset AppliedAt,
    DateTimeOffset? ExpiresAt) : ICommand;
