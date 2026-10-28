namespace NoMoreTears.Modules.Identity.Contracts.Constants;

public static class EndpointName
{
    public const string Register = nameof(Register);

    public const string Login = nameof(Login);

    public const string ConfirmEmail = nameof(ConfirmEmail);

    public const string ResendEmailConfirmation = nameof(ResendEmailConfirmation);

    public const string VerifyMfa = nameof(VerifyMfa);

    public const string Refresh = nameof(Refresh);

    public const string Logout = nameof(Logout);

    public const string LogoutAll = nameof(LogoutAll);

    public const string ForgotPassword = nameof(ForgotPassword);

    public const string ResetPassword = nameof(ResetPassword);

    public const string GetCurrentUser = nameof(GetCurrentUser);

    public const string VerifyPassword = nameof(VerifyPassword);

    public const string ChangePassword = nameof(ChangePassword);

    public const string UpdateMfa = nameof(UpdateMfa);

    public const string GetSessions = nameof(GetSessions);

    public const string RevokeSession = nameof(RevokeSession);

    public const string UpdateRoles = nameof(UpdateRoles);

    public const string DeactivateUser = nameof(DeactivateUser);

    public const string ReactivateUser = nameof(ReactivateUser);
}