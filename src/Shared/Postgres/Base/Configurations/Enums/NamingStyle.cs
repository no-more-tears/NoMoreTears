namespace NoMoreTears.Shared.Postgres.Configurations.Enums;

/// <summary>
///    Represents the naming convention for database objects.
///    I hope everyone already knows the naming convention, I'm too lazy to document it.
/// </summary>
public enum NamingStyle
{
    Default, // PascalCase
    LowerCase,
    UpperCase,
    SnakeCase,
    UpperSnakeCase,
    CamelCase
}