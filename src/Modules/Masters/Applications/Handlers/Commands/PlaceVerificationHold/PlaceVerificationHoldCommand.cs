namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.PlaceVerificationHold;

public sealed record PlaceVerificationHoldCommand(
    Guid MasterId,
    string Reason) : ICommand<MasterResponse>;
