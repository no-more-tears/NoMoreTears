using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Primitives.Commands;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Login;

public sealed record LoginCommand(
    string Identifier,
    string Password,
    ClientContext Context) : ICommand<LoginResponse>;