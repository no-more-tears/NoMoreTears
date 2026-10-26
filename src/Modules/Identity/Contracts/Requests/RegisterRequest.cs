namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record RegisterRequest(
    string Email,
    string? Username,
    string Password);
