namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetAvatar;

/// <summary>
///     Returns the profile photo of a master to any authenticated user.
/// </summary>
public sealed record GetAvatarQuery(
    Guid MasterId) : IQuery<FileContentResponse>;
