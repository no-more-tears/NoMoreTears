namespace NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Lengths of technical columns that have no rule in the domain.
/// </summary>
internal static class ColumnLengths
{
    /// <summary>
    ///     Enum values stored as text.
    /// </summary>
    public const int Enum = 32;

    /// <summary>
    ///     Media types, for example <c>application/pdf</c>.
    /// </summary>
    public const int ContentType = 100;
}