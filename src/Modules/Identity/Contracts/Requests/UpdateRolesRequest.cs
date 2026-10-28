using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Identity.Contracts.Requests;

public sealed record UpdateRolesRequest(
    IReadOnlyCollection<UserRole> Roles);