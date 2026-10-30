using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Primitives.Attributes;
using NoMoreTears.Shared.CQS.Primitives.Commands;
using NoMoreTears.Shared.Providers.Identity.Contracts.Common;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyMfa;

public sealed record VerifyMfaCommand(
    string ChallengeToken,
    [property: SensitiveData] string Code,
    ClientContext Context) : ICommand<TokenResponse>;
