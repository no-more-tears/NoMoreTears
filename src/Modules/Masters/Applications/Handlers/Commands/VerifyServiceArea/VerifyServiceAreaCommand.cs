namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyServiceArea;

public sealed record VerifyServiceAreaCommand(
    Guid ReviewerId,
    Guid MasterId) : ICommand<MasterResponse>;
