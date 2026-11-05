using System.Globalization;
using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     First and last name of a master as confirmed by the identity document.
/// </summary>
public sealed record FullName
{
    /// <summary>
    ///     First name.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    ///     Last name.
    /// </summary>
    public string LastName { get; }

    private FullName(
        string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>
    ///     Creates a full name.
    ///     Each part starts with a letter and contains only letters, spaces, hyphens and apostrophes.
    /// </summary>
    public static FullName Of(
        string firstName, 
        string lastName)
    {
        return new FullName(
            NormalizePart(firstName, "First name"),
            NormalizePart(lastName, "Last name"));
    }

    /// <inheritdoc />
    public override string ToString() => $"{FirstName} {LastName}";

    private static string NormalizePart(string value, string partName)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0 || normalized.Length > MasterRules.MaxNamePartLength)
        {
            throw new InvalidFullNameException(
                $"{partName} must contain from 1 to {MasterRules.MaxNamePartLength} characters.");
        }

        if (!char.IsLetter(normalized[0]) || !normalized.All(IsAllowedCharacter))
        {
            throw new InvalidFullNameException(
                $"{partName} must start with a letter and contain only letters, spaces, hyphens and apostrophes.");
        }

        return normalized;
    }

    private static bool IsAllowedCharacter(char character)
    {
        return char.IsLetter(character)
            || CharUnicodeInfo.GetUnicodeCategory(character) is UnicodeCategory.NonSpacingMark
            || character is ' ' or '-' or '\'' or '\u2019' or '\u02BC';
    }
}