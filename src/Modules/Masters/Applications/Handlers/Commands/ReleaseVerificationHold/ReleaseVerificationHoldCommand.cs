namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ReleaseVerificationHold;

public sealed record ReleaseVerificationHoldCommand(
    Guid MasterId) : ICommand<MasterResponse>;
