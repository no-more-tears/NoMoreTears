using NoMoreTears.Modules.Masters.Contracts.Requests;
using NoMoreTears.Modules.Masters.Domain.Exceptions;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;

namespace NoMoreTears.Modules.Masters.Applications.Services.Mappings;

/// <summary>
///     Manual mapping of requests to value objects; the factories of the value objects enforce the domain rules.
/// </summary>
public static class RequestMappings
{
    public static ServiceArea ToServiceArea(this ServiceAreaRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var hasCircle = request.Center is not null || request.RadiusInKilometers is not null;
        var hasPolygon = request.Boundary is { Count: > 0 };

        if (hasCircle == hasPolygon)
        {
            throw new InvalidServiceAreaException("Service area must be either a circle (center and radius) or a polygon.");
        }

        if (hasPolygon)
        {
            return ServiceArea.Of([.. request.Boundary!.Select(vertex => Coordinates.Of(vertex.Latitude, vertex.Longitude))]);
        }

        if (request.Center is null || request.RadiusInKilometers is null)
        {
            throw new InvalidServiceAreaException("A circular service area needs both a center and a radius.");
        }

        return ServiceArea.Of(
            Coordinates.Of(request.Center.Latitude, request.Center.Longitude),
            request.RadiusInKilometers.Value);
    }

    public static WorkSchedule ToWorkSchedule(this WorkScheduleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return WorkSchedule.Of(
            request.TimeZoneId,
            [.. (request.Hours ?? []).Select(interval => WorkingHours.Of(interval.Day, interval.Start, interval.End))]);
    }
}