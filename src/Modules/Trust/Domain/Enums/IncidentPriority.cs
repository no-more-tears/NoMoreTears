namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Priority of an incident review;
///     complaints of clients with many rejected complaints get a lower one.
/// </summary>
public enum IncidentPriority
{
    Normal,
    Low
}