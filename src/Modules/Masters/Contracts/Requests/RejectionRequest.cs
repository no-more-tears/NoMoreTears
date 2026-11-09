namespace NoMoreTears.Modules.Masters.Contracts.Requests;

/// <summary>
///     Rejects a review item with a reason shown to the master.
/// </summary>
public sealed record RejectionRequest(
    string Reason);
