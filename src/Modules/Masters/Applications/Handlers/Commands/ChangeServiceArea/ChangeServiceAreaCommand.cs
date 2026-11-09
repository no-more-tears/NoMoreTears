using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Modules.Masters.Contracts.Requests;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeServiceArea;

public sealed record ChangeServiceAreaCommand(
    Guid MasterId,
    ServiceAreaRequest ServiceArea) : ICommand<MasterResponse>, IMasterOwnedRequest;
