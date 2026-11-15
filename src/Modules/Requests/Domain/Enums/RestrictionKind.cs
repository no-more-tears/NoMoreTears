namespace NoMoreTears.Modules.Requests.Domain.Enums;

/// <summary>
///     Kind of a restriction of publication imposed on a client by Trust.
/// </summary>
public enum RestrictionKind
{
    /// <summary>
    ///     The client can have only one published request (sanction step 2 or the client abuse measure).
    /// </summary>
    PublicationLimit,

    /// <summary>
    ///     The client cannot publish requests (sanction step 3 or a blocked account).
    /// </summary>
    PublicationBlock
}