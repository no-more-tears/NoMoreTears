using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ReactivateUser;

public sealed record ReactivateUserCommand(
    Guid UserId) : ICommand;
