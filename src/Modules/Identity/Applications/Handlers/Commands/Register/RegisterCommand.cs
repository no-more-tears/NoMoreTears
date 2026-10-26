using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string? Username,
    string Password) : ICommand<Guid>;