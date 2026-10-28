using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : ICommand;
