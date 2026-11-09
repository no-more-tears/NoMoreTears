namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMasterCategory;

public sealed record RejectMasterCategoryCommand(
    Guid ReviewerId,
    Guid MasterId,
    Guid CategoryId,
    string Reason) : ICommand<MasterResponse>;
