using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Geographic zone served by a master: a radius around a center or a polygon.
/// </summary>
public sealed record ServiceArea
{
    private const double EarthRadiusInKilometers = 6371.0;

    /// <summary>
    ///     Shape of the area.
    /// </summary>
    public ServiceAreaKind Kind { get; }

    /// <summary>
    ///     Center of a radius area.
    /// </summary>
    public Coordinates? Center { get; }

    /// <summary>
    ///     Radius in kilometers.
    /// </summary>
    public double? RadiusInKilometers { get; }

    /// <summary>
    ///     Vertices of a polygon area, empty for a radius area.
    /// </summary>
    public IReadOnlyList<Coordinates> Boundary { get; }

    private ServiceArea(
        ServiceAreaKind kind,
        Coordinates? center,
        double? radiusInKilometers,
        IReadOnlyList<Coordinates> boundary)
    {
        Kind = kind;
        Center = center;
        RadiusInKilometers = radiusInKilometers;
        Boundary = boundary;
    }

    /// <summary>
    ///     Creates a radius area.
    /// </summary>
    public static ServiceArea Of(Coordinates center, double radiusInKilometers)
    {
        ArgumentNullException.ThrowIfNull(center);

        if (!double.IsFinite(radiusInKilometers)
            || radiusInKilometers <= 0
            || radiusInKilometers > MasterRules.MaxServiceRadiusInKilometers)
        {
            throw new InvalidServiceAreaException(
                $"Service radius must be greater than 0 and at most {MasterRules.MaxServiceRadiusInKilometers} km.");
        }

        return new ServiceArea(ServiceAreaKind.Radius, center, radiusInKilometers, []);
    }

    /// <summary>
    ///     Creates a polygon area.
    /// </summary>
    public static ServiceArea Of(IReadOnlyCollection<Coordinates> boundary)
    {
        ArgumentNullException.ThrowIfNull(boundary);

        if (boundary.Distinct().Count() < MasterRules.MinPolygonVertices
            || boundary.Count > MasterRules.MaxPolygonVertices)
        {
            throw new InvalidServiceAreaException(
                $"Service polygon must contain from {MasterRules.MinPolygonVertices} to {MasterRules.MaxPolygonVertices} distinct vertices.");
        }

        return new ServiceArea(ServiceAreaKind.Polygon, null, null, [.. boundary]);
    }

    /// <summary>
    ///     Returns whether the given point lies inside the area.
    /// </summary>
    public bool Covers(Coordinates point)
    {
        ArgumentNullException.ThrowIfNull(point);

        return Kind is ServiceAreaKind.Radius
            ? DistanceInKilometers(Center!, point) <= RadiusInKilometers
            : IsInsidePolygon(point);
    }

    /// <inheritdoc />
    public bool Equals(ServiceArea? other)
    {
        return other is not null
            && Kind == other.Kind
            && Equals(Center, other.Center)
            && Nullable.Equals(RadiusInKilometers, other.RadiusInKilometers)
            && Boundary.SequenceEqual(other.Boundary);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(Kind);
        hash.Add(Center);
        hash.Add(RadiusInKilometers);

        foreach (var vertex in Boundary)
        {
            hash.Add(vertex);
        }

        return hash.ToHashCode();
    }

    private bool IsInsidePolygon(Coordinates point)
    {
        var isInside = false;
        var previous = Boundary.Count - 1;

        for (var current = 0; current < Boundary.Count; current++)
        {
            var a = Boundary[current];
            var b = Boundary[previous];

            if ((a.Latitude > point.Latitude) != (b.Latitude > point.Latitude)
                && point.Longitude < ((b.Longitude - a.Longitude) * (point.Latitude - a.Latitude) / (b.Latitude - a.Latitude)) + a.Longitude)
            {
                isInside = !isInside;
            }

            previous = current;
        }

        return isInside;
    }

    private static double DistanceInKilometers(Coordinates from, Coordinates to)
    {
        var latitudeDelta = ToRadians(to.Latitude - from.Latitude);
        var longitudeDelta = ToRadians(to.Longitude - from.Longitude);
        var haversine = (Math.Sin(latitudeDelta / 2) * Math.Sin(latitudeDelta / 2))
            + (Math.Cos(ToRadians(from.Latitude)) * Math.Cos(ToRadians(to.Latitude))
                * Math.Sin(longitudeDelta / 2) * Math.Sin(longitudeDelta / 2));

        return 2 * EarthRadiusInKilometers * Math.Asin(Math.Min(1, Math.Sqrt(haversine)));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}