namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Geocoded address of the work: the text shown to the master and its coordinates.
/// </summary>
public sealed record AddressRequest(
    string Value,
    CoordinatesRequest Coordinates);