namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Status of a temporary measure;
///     no automatic measure is final without a human.
/// </summary>
public enum MeasureStatus
{
    PendingReview,
    Escalated,
    Confirmed,
    Lifted,
    Expired
}