namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record LoginRequest(
    string Identifier,
    string Password);