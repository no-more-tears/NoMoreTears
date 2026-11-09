using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Results;

/// <summary>
///     Profile after a sanction was applied or lifted.
/// </summary>
public sealed record SanctionOutcome(
    MasterResponse Master,
    SanctionResponse Sanction,
    bool VerificationChanged);