using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.AcceptRequest;

/// <summary>
///     Accepts a published request with the expected arrival interval; acceptance is exclusive (FR-DSP-04, FR-DSP-05).
/// </summary>
public sealed record AcceptRequestCommand(
    Guid MasterId,
    Guid RequestId,
    int ArrivalIntervalInHours) : ICommand<RequestResponse>, IRequestMasterRequest;