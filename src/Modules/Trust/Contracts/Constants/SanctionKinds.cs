namespace NoMoreTears.Modules.Trust.Contracts.Constants;

/// <summary>
///     Sanctions of the ladder in Trust events.
/// </summary>
public static class SanctionKinds
{
    public const string Warning = nameof(Warning);

    public const string Restriction = nameof(Restriction);

    public const string Suspension = nameof(Suspension);

    public const string Deverification = nameof(Deverification);

    public const string PublicationRestriction = nameof(PublicationRestriction);

    public const string PublicationBlock = nameof(PublicationBlock);

    public const string AccountBlock = nameof(AccountBlock);
}
