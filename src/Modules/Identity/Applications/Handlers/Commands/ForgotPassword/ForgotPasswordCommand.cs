using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string Identifier) : ICommand;
