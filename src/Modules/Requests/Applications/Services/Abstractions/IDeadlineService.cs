using NoMoreTears.Modules.Requests.Contracts.Responses;

namespace NoMoreTears.Modules.Requests.Applications.Services.Abstractions;

/// <summary>
///     Transitions that happen when a deadline passes.
/// </summary>
public interface IDeadlineService
{
    /// <summary>
    ///     Expires published requests whose acceptance deadline has passed.
    /// </summary>
    Task<IReadOnlyList<RequestResponse>> ExpireOverdueAsync(
        int batchSize,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Completes requests whose confirmation period has passed without an objection.
    /// </summary>
    Task<IReadOnlyList<RequestResponse>> CompleteOverdueAsync(
        int batchSize,
        CancellationToken cancellationToken);
}