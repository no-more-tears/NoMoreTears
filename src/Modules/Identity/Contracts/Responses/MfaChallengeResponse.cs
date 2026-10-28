namespace NoMoreTears.Modules.Identity.Contracts.Responses;

public sealed record MfaChallengeResponse(
    string ChallengeToken,
    DateTimeOffset ExpiresAt);