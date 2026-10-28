namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record UpdateMfaRequest(
    string Password,
    bool Enabled);