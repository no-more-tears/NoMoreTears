using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.AddMasterCategory;

public sealed record AddMasterCategoryCommand(
    Guid MasterId,
    Guid CategoryId) : ICommand<MasterResponse>, IMasterOwnedRequest;
