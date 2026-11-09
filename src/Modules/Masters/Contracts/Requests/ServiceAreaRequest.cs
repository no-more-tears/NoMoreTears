namespace NoMoreTears.Modules.Masters.Contracts.Requests;

/// <summary>
///     Zone served by a master: a circle (<see cref="Center"/> + <see cref="RadiusInKilometers"/>)
///     or a polygon (<see cref="Boundary"/>). Exactly one form must be given.
/// </summary>
public sealed record ServiceAreaRequest(
    CoordinatesRequest? Center,
    double? RadiusInKilometers,
    IReadOnlyList<CoordinatesRequest>? Boundary);
