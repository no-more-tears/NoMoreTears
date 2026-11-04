using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Configuration;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization;

/// <summary>
///     Web module that answers in the language asked by the client with the <c>Accept-Language</c> header
///     or the <c>culture</c> query parameter.
/// </summary>
public sealed class LocalizationWebModule : IWebModule
{
    /// <inheritdoc />
    public int Order => WebModuleOrder.Localization;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        var options = context.Configuration
            .GetSection(LocalizationOptions.SectionName)
            .Get<LocalizationOptions>()
            ?? new LocalizationOptions();

        var defaultCulture = CultureResolver.Resolve(options.DefaultCulture, LocalizationOptions.SectionName);

        var supportedCultures = options.SupportedCultures
            .Select(name => CultureResolver.Resolve(name, LocalizationOptions.SectionName))
            .Prepend(defaultCulture)
            .DistinctBy(culture => culture.Name)
            .ToList();

        var catalog = TranslationCatalog.Load([typeof(LocalizationWebModule).Assembly]);

        context.Services.AddSingleton<IMessageTranslator>(new MessageTranslator(catalog));

        context.Services.Configure<RequestLocalizationOptions>(localization =>
        {
            localization.DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture);
            localization.SupportedCultures = [defaultCulture];
            localization.SupportedUICultures = supportedCultures;
            localization.FallBackToParentUICultures = true;
            localization.ApplyCurrentCultureToResponseHeaders = true;
            localization.RequestCultureProviders =
            [
                new QueryStringRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        context.Services.PostConfigure<ProblemDetailsOptions>(problemDetailsOptions =>
        {
            var customize = problemDetailsOptions.CustomizeProblemDetails;

            problemDetailsOptions.CustomizeProblemDetails = problemContext =>
            {
                customize?.Invoke(problemContext);

                var translator = problemContext.HttpContext.RequestServices.GetRequiredService<IMessageTranslator>();
                var culture = CultureInfo.CurrentUICulture;
                var problem = problemContext.ProblemDetails;

                problem.Title = problem.Title is null ? null : translator.Translate(problem.Title, culture);
                problem.Detail = problem.Detail is null ? null : translator.Translate(problem.Detail, culture);

                // The exception handler clears the headers set by the localization middleware, so they are restored here.
                var response = problemContext.HttpContext.Response;

                if (!response.HasStarted)
                {
                    response.Headers.ContentLanguage = culture.Name;
                }
            };
        });
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
        // Mounted before exception handling, so error responses are written in the requested language.
        app.UseRequestLocalization();
    }
}