using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeFullName;

public sealed record ChangeFullNameCommand(
    Guid MasterId,
    string FirstName,
    string LastName) : ICommand<MasterResponse>, IMasterOwnedRequest;
