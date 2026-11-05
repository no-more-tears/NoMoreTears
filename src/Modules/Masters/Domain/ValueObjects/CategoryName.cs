using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Name of a service category in the catalog.
/// </summary>
public sealed record CategoryName
{
    /// <summary>
    ///     Trimmed name.
    /// </summary>
    public string Value { get; }

    private CategoryName(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a category name.
    /// </summary>
    public static CategoryName Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length < MasterRules.MinCategoryNameLength
            || normalized.Length > MasterRules.MaxCategoryNameLength)
        {
            throw new InvalidCategoryException(
                $"Category name must contain from {MasterRules.MinCategoryNameLength} to {MasterRules.MaxCategoryNameLength} characters.");
        }

        return new CategoryName(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}