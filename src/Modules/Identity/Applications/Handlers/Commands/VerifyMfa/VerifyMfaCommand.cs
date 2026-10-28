using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Primitives.Commands;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyMfa;

public sealed record VerifyMfaCommand(
    string ChallengeToken,
    string Code,
    ClientContext Context) : ICommand<TokenResponse>;
