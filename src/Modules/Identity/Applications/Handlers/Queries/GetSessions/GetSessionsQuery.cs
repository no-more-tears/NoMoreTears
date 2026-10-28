using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Primitives.Queries;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Queries.GetSessions;

public sealed record GetSessionsQuery(
    Guid UserId,
    Guid? CurrentSessionId) : IQuery<IReadOnlyList<SessionResponse>>;
