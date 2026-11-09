using System.Text.Json;
using System.Text.Json.Serialization;
using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;

namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts.Conversions;

/// <summary>
///     Converts value objects with nested collections to <c>jsonb</c> and back.
///     Restoring goes through the factories of the value objects, so stored data is validated by the domain.
/// </summary>
internal static class ValueObjectJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static string SerializeServiceArea(ServiceArea area)
    {
        var data = new ServiceAreaData(
            area.Kind,
            area.Center is null ? null : new CoordinatesData(area.Center.Latitude, area.Center.Longitude),
            area.RadiusInKilometers,
            [.. area.Boundary.Select(vertex => new CoordinatesData(vertex.Latitude, vertex.Longitude))]);

        return JsonSerializer.Serialize(data, Options);
    }

    public static ServiceArea DeserializeServiceArea(string json)
    {
        var data = JsonSerializer.Deserialize<ServiceAreaData>(json, Options)
            ?? throw new JsonException("Service area is empty.");

        return data.Kind is ServiceAreaKind.Radius
            ? ServiceArea.Of(
                Coordinates.Of(data.Center!.Latitude, data.Center.Longitude),
                data.RadiusInKilometers!.Value)
            : ServiceArea.Of([.. data.Boundary.Select(vertex => Coordinates.Of(vertex.Latitude, vertex.Longitude))]);
    }

    public static string SerializeWorkSchedule(WorkSchedule schedule)
    {
        var data = new WorkScheduleData(
            schedule.TimeZoneId,
            [.. schedule.Hours.Select(interval => new WorkingHoursData(interval.Day, interval.Start, interval.End))]);

        return JsonSerializer.Serialize(data, Options);
    }

    public static WorkSchedule DeserializeWorkSchedule(string json)
    {
        var data = JsonSerializer.Deserialize<WorkScheduleData>(json, Options)
            ?? throw new JsonException("Work schedule is empty.");

        return WorkSchedule.Of(
            data.TimeZoneId,
            [.. data.Hours.Select(interval => WorkingHours.Of(interval.Day, interval.Start, interval.End))]);
    }

    private sealed record CoordinatesData(double Latitude, double Longitude);

    private sealed record ServiceAreaData(
        ServiceAreaKind Kind,
        CoordinatesData? Center,
        double? RadiusInKilometers,
        CoordinatesData[] Boundary);

    private sealed record WorkingHoursData(DayOfWeek Day, TimeOnly Start, TimeOnly End);

    private sealed record WorkScheduleData(string TimeZoneId, WorkingHoursData[] Hours);
}