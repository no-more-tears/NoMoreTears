using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RefuseRequest;

/// <summary>
///     Records the refusal of the assigned master with a reason (FR-REQ-07).
/// </summary>
public sealed record RefuseRequestCommand(
    Guid MasterId,
    Guid RequestId,
    string Reason) : ICommand<RequestResponse>, IRequestMasterRequest;