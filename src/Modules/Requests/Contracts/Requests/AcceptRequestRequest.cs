namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Accepts a published request with the expected arrival interval in hours (2, 6 or 24).
/// </summary>
public sealed record AcceptRequestRequest(
    int ArrivalIntervalInHours);