namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record ForgotPasswordRequest(
    string Identifier);