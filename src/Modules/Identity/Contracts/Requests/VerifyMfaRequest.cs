namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record VerifyMfaRequest(
    string ChallengeToken,
    string Code);