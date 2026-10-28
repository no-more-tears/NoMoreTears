namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record ResetPasswordRequest(
    Guid UserId,
    string Token,
    string NewPassword);