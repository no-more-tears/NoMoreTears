namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Geographic point in degrees (WGS 84).
/// </summary>
public sealed record CoordinatesRequest(
    double Latitude,
    double Longitude);