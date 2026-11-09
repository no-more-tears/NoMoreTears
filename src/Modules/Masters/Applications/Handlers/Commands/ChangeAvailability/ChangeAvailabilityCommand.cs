using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeAvailability;

public sealed record ChangeAvailabilityCommand(
    Guid MasterId,
    bool IsAvailable) : ICommand<MasterResponse>, IMasterOwnedRequest;
