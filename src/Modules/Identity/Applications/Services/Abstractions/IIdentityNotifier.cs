using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Modules.Identity.Applications.Services.Abstractions;

public interface IIdentityNotifier
{
    Task SendEmailConfirmationAsync(
        string email, 
        UserActionTicket ticket, 
        CancellationToken cancellationToken);

    Task SendPasswordResetAsync(
        string email,
        UserActionTicket ticket,
        CancellationToken cancellationToken);

    Task SendMfaCodeAsync(
        MfaChallenge challenge,
        CancellationToken cancellationToken);
}