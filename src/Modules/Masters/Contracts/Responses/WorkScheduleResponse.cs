namespace NoMoreTears.Modules.Masters.Contracts.Responses;

public sealed record WorkScheduleResponse(
    string TimeZoneId,
    IReadOnlyList<WorkingHoursResponse> Hours);
