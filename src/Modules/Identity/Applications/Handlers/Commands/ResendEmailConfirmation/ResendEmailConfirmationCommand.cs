using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ResendEmailConfirmation;

public sealed record ResendEmailConfirmationCommand(
    string Identifier) : ICommand;
