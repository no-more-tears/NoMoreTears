namespace NoMoreTears.Modules.Masters.Contracts.Responses;

public sealed record WorkingHoursResponse(
    DayOfWeek Day,
    TimeOnly Start,
    TimeOnly End);
