namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectServiceArea;

public sealed record RejectServiceAreaCommand(
    Guid ReviewerId,
    Guid MasterId,
    string Reason) : ICommand<MasterResponse>;
