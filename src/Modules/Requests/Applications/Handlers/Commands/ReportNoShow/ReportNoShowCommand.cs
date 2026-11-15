using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ReportNoShow;

/// <summary>
///     Reports that the master did not arrive within the expected time and the buffer of 60 minutes (FR-REQ-07).
/// </summary>
public sealed record ReportNoShowCommand(
    Guid ClientId,
    Guid RequestId) : ICommand<RequestResponse>, IRequestClientRequest;