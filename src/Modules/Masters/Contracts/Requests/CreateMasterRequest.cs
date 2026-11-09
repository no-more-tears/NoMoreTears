namespace NoMoreTears.Modules.Masters.Contracts.Requests;

/// <summary>
///     Creates the master profile of the current user.
/// </summary>
public sealed record CreateMasterRequest(
    string FirstName,
    string LastName,
    IReadOnlyList<Guid> CategoryIds,
    ServiceAreaRequest ServiceArea,
    WorkScheduleRequest WorkSchedule);
