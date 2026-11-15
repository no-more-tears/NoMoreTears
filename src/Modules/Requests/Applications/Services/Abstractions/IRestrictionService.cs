namespace NoMoreTears.Modules.Requests.Applications.Services.Abstractions;

/// <summary>
///     Applies publication restrictions of Trust to clients.
/// </summary>
public interface IRestrictionService
{
    /// <summary>
    ///     Records a restriction; returns <c>false</c> when it is already known.
    /// </summary>
    Task<bool> ApplyAsync(
        Guid restrictionId,
        Guid clientId,
        string kind,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Lifts a restriction; returns <c>false</c> when it is unknown or already lifted.
    /// </summary>
    Task<bool> LiftAsync(
        Guid restrictionId,
        DateTimeOffset liftedAt,
        CancellationToken cancellationToken);
}