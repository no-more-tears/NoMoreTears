using NoMoreTears.Shared.RabbitMq.Contracts;

namespace NoMoreTears.Modules.Trust.Contracts.Events;

/// <summary>
///     An automatic rule created a temporary measure that waits for a human review (FR-TRS-07).
/// </summary>
/// <param name="MeasureId">Identifier of the measure; other modules keep it to lift the measure later.</param>
/// <param name="Rule">Automatic rule, also the reason code shown to the user, for example <c>DuplicateAccount</c>.</param>
/// <param name="Effects">Effects applied by other contexts, see <see cref="Constants.MeasureEffects"/>.</param>
public sealed record TemporaryMeasureAppliedEvent(
    Guid MeasureId,
    Guid SubjectId,
    string SubjectRole,
    string Rule,
    IReadOnlyList<string> Effects,
    DateTimeOffset AppliedAt,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset OccurredAt) : IntegrationEvent(OccurredAt);
