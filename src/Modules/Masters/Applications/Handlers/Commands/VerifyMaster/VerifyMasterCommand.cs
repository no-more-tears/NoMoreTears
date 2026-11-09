namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMaster;

public sealed record VerifyMasterCommand(
    Guid ReviewerId,
    Guid MasterId) : ICommand<MasterResponse>;
