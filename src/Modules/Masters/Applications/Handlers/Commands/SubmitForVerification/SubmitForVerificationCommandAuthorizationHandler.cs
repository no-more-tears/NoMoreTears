using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.SubmitForVerification;

public sealed class SubmitForVerificationCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : MasterOwnerAuthorizationHandler<SubmitForVerificationCommand>(currentUser);
