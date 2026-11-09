using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Modules.Masters.Contracts.Requests;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeWorkSchedule;

public sealed record ChangeWorkScheduleCommand(
    Guid MasterId,
    WorkScheduleRequest WorkSchedule) : ICommand<MasterResponse>, IMasterOwnedRequest;
