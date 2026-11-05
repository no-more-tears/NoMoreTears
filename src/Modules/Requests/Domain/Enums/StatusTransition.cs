namespace NoMoreTears.Modules.Requests.Domain.Enums;

/// <summary>
///     Business basis of a status change recorded in the request history.
/// </summary>
public enum StatusTransition
{
    Created,
    Published,
    Extended,
    Republished,
    Expired,
    Accepted,
    MarkedAsDone,
    CompletionConfirmed,
    CompletedAutomatically,
    CompletionObjected,
    NoShowReported,
    RefusedByMaster,
    CancelledBeforeAcceptance,
    CancelledAfterAcceptance,
    Disputed,
    DisputeSettled
}