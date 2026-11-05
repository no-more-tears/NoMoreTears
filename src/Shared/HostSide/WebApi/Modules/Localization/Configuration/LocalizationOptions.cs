namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Configuration;

/// <summary>
///     Configuration of the languages in which the API answers.
/// </summary>
public sealed class LocalizationOptions
{
    public const string SectionName = "Localization";

    /// <summary>
    ///     Language used when the client does not ask for a supported one.
    /// </summary>
    public string DefaultCulture { get; init; } = "en";

    /// <summary>
    ///     Languages a client can ask for with the <c>Accept-Language</c> header or the <c>culture</c> query parameter.
    /// </summary>
    public string[] SupportedCultures { get; init; } = ["en", "uk"];
}