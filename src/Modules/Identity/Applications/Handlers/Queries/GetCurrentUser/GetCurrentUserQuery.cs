using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Primitives.Queries;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(
    Guid UserId) : IQuery<UserResponse>;
