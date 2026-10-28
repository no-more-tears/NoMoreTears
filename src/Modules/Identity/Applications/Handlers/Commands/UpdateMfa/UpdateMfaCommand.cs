using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateMfa;

public sealed record UpdateMfaCommand(
    Guid UserId,
    string Password,
    bool Enabled) : ICommand;
