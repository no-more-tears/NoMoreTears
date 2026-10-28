namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record RefreshRequest(
    string RefreshToken);