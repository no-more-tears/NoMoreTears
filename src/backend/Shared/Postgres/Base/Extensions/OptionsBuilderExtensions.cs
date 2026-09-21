using Microsoft.EntityFrameworkCore;
using NoMoreTears.Shared.Postgres.Configurations.Enums;

namespace NoMoreTears.Shared.Postgres.Extensions;

/// <summary>
///     Extension methods for configuring options in the Postgres context.
/// </summary>
public static class OptionsBuilderExtensions
{
    /// <summary>
    ///     Configures the naming convention used for the database schema
    ///     (tables, columns, keys and indexes).
    /// </summary>
    /// <returns>The same builder instance for chaining.</returns>
    public static DbContextOptionsBuilder<TDbContext> UseNamingStyle<TDbContext>(
        this DbContextOptionsBuilder<TDbContext> optionsBuilder,
        NamingStyle namingStyle)
        where TDbContext : DbContext
    {
        return namingStyle switch
        {
            NamingStyle.Default => optionsBuilder,
            NamingStyle.LowerCase => optionsBuilder.UseLowerCaseNamingConvention(),
            NamingStyle.UpperCase => optionsBuilder.UseUpperCaseNamingConvention(),
            NamingStyle.SnakeCase => optionsBuilder.UseSnakeCaseNamingConvention(),
            NamingStyle.UpperSnakeCase => optionsBuilder.UseUpperSnakeCaseNamingConvention(),
            NamingStyle.CamelCase => optionsBuilder.UseCamelCaseNamingConvention(),
            _ => throw new NotSupportedException($"The naming style '{namingStyle}' is not supported.")
        };
    }
}