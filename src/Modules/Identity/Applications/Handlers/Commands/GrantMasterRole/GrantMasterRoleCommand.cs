using NoMoreTears.Shared.CQS.Primitives.Commands;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.GrantMasterRole;

/// <summary>
///     Grants the Master role to a user whose master profile was verified. Sent only by the system
///     (a consumer of the Masters events), so the command has no authorization handler and no endpoint.
/// </summary>
public sealed record GrantMasterRoleCommand(
    Guid UserId) : ICommand;
