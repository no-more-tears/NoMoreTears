using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Primitives.Commands;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Refresh;

public sealed record RefreshCommand(
    string RefreshToken,
    ClientContext Context) : ICommand<TokenResponse>;
