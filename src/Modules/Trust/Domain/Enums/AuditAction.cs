namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Action recorded in the immutable audit journal.
/// </summary>
public enum AuditAction
{
    IncidentOpened,
    IncidentTakenIntoReview,
    IncidentDecided,
    DecisionAppealed,
    AppealDecided,
    IncidentClosed,
    SanctionImposed,
    SanctionLifted,
    MeasureApplied,
    MeasureContested,
    MeasureEscalated,
    MeasureConfirmed,
    MeasureLifted,
    MeasureExpired
}