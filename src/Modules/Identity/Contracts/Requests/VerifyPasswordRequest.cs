namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record VerifyPasswordRequest(
    string Password);