namespace NoMoreTears.Modules.Masters.Contracts.Responses;

/// <summary>
///     Zone served by a master. <see cref="Kind"/> is <c>Radius</c> or <c>Polygon</c>.
/// </summary>
public sealed record ServiceAreaResponse(
    string Kind,
    CoordinatesResponse? Center,
    double? RadiusInKilometers,
    IReadOnlyList<CoordinatesResponse> Boundary);
