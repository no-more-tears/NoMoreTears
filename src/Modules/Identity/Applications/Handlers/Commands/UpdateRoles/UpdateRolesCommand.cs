using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateRoles;

public sealed record UpdateRolesCommand(
    Guid UserId,
    IReadOnlyCollection<UserRole> Roles) : ICommand;
