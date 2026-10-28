using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyPassword;

public sealed record VerifyPasswordCommand(
    Guid UserId,
    string Password) : ICommand;
