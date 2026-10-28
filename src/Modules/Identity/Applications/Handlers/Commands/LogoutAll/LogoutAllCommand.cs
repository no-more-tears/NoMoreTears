using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.LogoutAll;

public sealed record LogoutAllCommand(
    Guid UserId) : ICommand;
