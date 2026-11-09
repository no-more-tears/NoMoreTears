namespace NoMoreTears.Modules.Masters.Contracts.Responses;

public sealed record VerificationHoldResponse(
    string Reason,
    DateTimeOffset PlacedAt);
