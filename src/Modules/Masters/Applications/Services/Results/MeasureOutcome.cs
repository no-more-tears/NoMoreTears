using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Results;

/// <summary>
///     Profile after a temporary measure was applied or lifted.
/// </summary>
public sealed record MeasureOutcome(
    MasterResponse Master,
    SanctionResponse? Suspension,
    bool HoldChanged);