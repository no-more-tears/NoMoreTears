using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveMasterCategory;

public sealed record RemoveMasterCategoryCommand(
    Guid MasterId,
    Guid CategoryId) : ICommand<MasterResponse>, IMasterOwnedRequest;
