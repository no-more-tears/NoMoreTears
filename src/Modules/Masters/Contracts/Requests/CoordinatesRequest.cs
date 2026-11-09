namespace NoMoreTears.Modules.Masters.Contracts.Requests;

/// <summary>
///     A point given by latitude and longitude in degrees.
/// </summary>
public sealed record CoordinatesRequest(
    double Latitude,
    double Longitude);
