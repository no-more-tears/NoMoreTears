namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMaster;

public sealed record RejectMasterCommand(
    Guid ReviewerId,
    Guid MasterId,
    string Reason) : ICommand<MasterResponse>;
