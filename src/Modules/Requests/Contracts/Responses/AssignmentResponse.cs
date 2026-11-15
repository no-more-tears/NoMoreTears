namespace NoMoreTears.Modules.Requests.Contracts.Responses;

/// <summary>
///     Master who accepted the current publication and the expected arrival.
/// </summary>
public sealed record AssignmentResponse(
    Guid MasterId,
    DateTimeOffset AcceptedAt,
    DateTimeOffset ExpectedArrivalAt,
    DateTimeOffset NoShowReportableFrom);