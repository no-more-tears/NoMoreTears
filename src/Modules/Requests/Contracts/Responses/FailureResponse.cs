namespace NoMoreTears.Modules.Requests.Contracts.Responses;

/// <summary>
///     Failure of the current publication. <see cref="Kind"/> is <c>NoShow</c> or <c>MasterRefusal</c>.
/// </summary>
public sealed record FailureResponse(
    string Kind,
    Guid MasterId,
    string? Reason);