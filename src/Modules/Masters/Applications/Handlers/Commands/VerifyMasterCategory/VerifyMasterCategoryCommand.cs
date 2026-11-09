namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMasterCategory;

public sealed record VerifyMasterCategoryCommand(
    Guid ReviewerId,
    Guid MasterId,
    Guid CategoryId) : ICommand<MasterResponse>;
