using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ConfirmEmail;

public sealed record class ConfirmEmailCommand(
    Guid UserId,
    string Token) : ICommand;