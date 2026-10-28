namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);