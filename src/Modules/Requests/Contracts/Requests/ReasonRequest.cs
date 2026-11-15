namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Reason of an action, recorded in the history of the request.
/// </summary>
public sealed record ReasonRequest(
    string Reason);