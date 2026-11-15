using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ReportNoShow;

public sealed class ReportNoShowCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<ReportNoShowCommand>(currentUser);