namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Role of whoever performed an audited action.
/// </summary>
public enum ActorRole
{
    System,
    Client,
    Master,
    PlatformSupport,
    PlatformAdministrator
}