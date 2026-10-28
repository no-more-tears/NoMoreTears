using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.RevokeSession;

public sealed record RevokeSessionCommand(
    Guid UserId,
    Guid SessionId) : ICommand;
