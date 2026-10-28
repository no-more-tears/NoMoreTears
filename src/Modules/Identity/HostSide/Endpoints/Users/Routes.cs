namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Users;

public static class Routes
{
    public const string Base = "/users";

    public const string Me = $"{Base}/me";

    public const string VerifyPassword = $"{Me}/verify-password";

    public const string ChangePassword = $"{Me}/change-password";

    public const string Mfa = $"{Me}/mfa";

    public const string Sessions = $"{Me}/sessions";

    public const string Session = $"{Sessions}/{{sessionId:guid}}";

    public const string Roles = $"{Base}/{{userId:guid}}/roles";

    public const string Deactivate = $"{Base}/{{userId:guid}}/deactivate";

    public const string Reactivate = $"{Base}/{{userId:guid}}/reactivate";
}