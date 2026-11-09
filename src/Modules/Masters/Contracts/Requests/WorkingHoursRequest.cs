namespace NoMoreTears.Modules.Masters.Contracts.Requests;

/// <summary>
///     A working interval within one day of the week, in the time zone of the schedule.
/// </summary>
public sealed record WorkingHoursRequest(
    DayOfWeek Day,
    TimeOnly Start,
    TimeOnly End);
