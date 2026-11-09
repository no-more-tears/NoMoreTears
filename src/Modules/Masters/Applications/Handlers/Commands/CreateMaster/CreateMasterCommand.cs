using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Modules.Masters.Contracts.Requests;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateMaster;

/// <summary>
///     Creates the master profile of the current user; the profile id is the user id.
/// </summary>
public sealed record CreateMasterCommand(
    Guid MasterId,
    string FirstName,
    string LastName,
    IReadOnlyList<Guid> CategoryIds,
    ServiceAreaRequest ServiceArea,
    WorkScheduleRequest WorkSchedule) : ICommand<MasterResponse>, IMasterOwnedRequest;
