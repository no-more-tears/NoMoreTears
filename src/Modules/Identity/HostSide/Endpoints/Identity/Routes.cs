namespace NoMoreTears.Modules.Identity.HostSide.Endpoints.Identity;

public static class Routes
{
    public const string Base = "/identity";

    public const string Register = $"{Base}/register";

    public const string Login = $"{Base}/login";

    public const string ConfirmEmail = $"{Base}/confirm-email";

    public const string ResendEmailConfirmation = $"{Base}/resend-email-confirmation";

    public const string VerifyMfa = $"{Base}/verify-mfa";

    public const string Refresh = $"{Base}/refresh";

    public const string Logout = $"{Base}/logout";

    public const string LogoutAll = $"{Base}/logout-all";

    public const string ForgotPassword = $"{Base}/forgot-password";

    public const string ResetPassword = $"{Base}/reset-password";
}