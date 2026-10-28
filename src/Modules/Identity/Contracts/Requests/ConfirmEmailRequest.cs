namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record ConfirmEmailRequest(
    Guid UserId,
    string Token);