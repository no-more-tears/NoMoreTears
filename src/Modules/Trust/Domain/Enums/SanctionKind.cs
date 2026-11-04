namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Sanction of the ladder for a master or a client.
/// </summary>
public enum SanctionKind
{
    Warning,
    Restriction,
    Suspension,
    Deverification,
    PublicationRestriction,
    PublicationBlock,
    AccountBlock
}