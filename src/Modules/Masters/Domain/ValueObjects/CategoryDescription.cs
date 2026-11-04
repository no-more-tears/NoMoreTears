using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Description of a service category in the catalog.
/// </summary>
public sealed record CategoryDescription
{
    /// <summary>
    ///     Trimmed description.
    /// </summary>
    public string Value { get; }

    private CategoryDescription(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a category description.
    /// </summary>
    public static CategoryDescription Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0 || normalized.Length > MasterRules.MaxCategoryDescriptionLength)
        {
            throw new InvalidCategoryException(
                $"Category description must contain from 1 to {MasterRules.MaxCategoryDescriptionLength} characters.");
        }

        return new CategoryDescription(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}