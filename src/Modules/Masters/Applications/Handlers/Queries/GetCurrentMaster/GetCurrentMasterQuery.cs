using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCurrentMaster;

public sealed record GetCurrentMasterQuery(
    Guid MasterId) : IQuery<MasterResponse>, IMasterOwnedRequest;
