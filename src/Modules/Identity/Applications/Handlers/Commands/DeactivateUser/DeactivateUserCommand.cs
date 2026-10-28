using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(
    Guid UserId) : ICommand;
