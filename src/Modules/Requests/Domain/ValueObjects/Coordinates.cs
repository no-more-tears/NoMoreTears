using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.ValueObjects;

/// <summary>
///     Geographic point produced by geocoding an address.
/// </summary>
public sealed record Coordinates
{
    /// <summary>
    ///     Latitude in degrees, from -90 to 90.
    /// </summary>
    public double Latitude { get; }

    /// <summary>
    ///     Longitude in degrees, from -180 to 180.
    /// </summary>
    public double Longitude { get; }

    private Coordinates(
        double latitude,
        double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    ///     Creates coordinates from latitude and longitude in degrees.
    /// </summary>
    public static Coordinates Of(double latitude, double longitude)
    {
        if (!double.IsFinite(latitude) || latitude is < -90 or > 90)
        {
            throw new InvalidAddressException("Latitude must be between -90 and 90 degrees.");
        }

        if (!double.IsFinite(longitude) || longitude is < -180 or > 180)
        {
            throw new InvalidAddressException("Longitude must be between -180 and 180 degrees.");
        }

        return new Coordinates(latitude, longitude);
    }
}