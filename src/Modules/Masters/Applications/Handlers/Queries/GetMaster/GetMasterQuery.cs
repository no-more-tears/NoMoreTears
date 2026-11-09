namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMaster;

public sealed record GetMasterQuery(
    Guid MasterId) : IQuery<MasterResponse>;
