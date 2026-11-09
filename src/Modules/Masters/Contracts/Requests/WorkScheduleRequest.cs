namespace NoMoreTears.Modules.Masters.Contracts.Requests;

/// <summary>
///     Weekly work schedule in an IANA time zone, for example <c>Europe/Kyiv</c>.
/// </summary>
public sealed record WorkScheduleRequest(
    string TimeZoneId,
    IReadOnlyList<WorkingHoursRequest> Hours);
