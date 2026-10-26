using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Applications.Services.Configuration;
using NoMoreTears.Modules.Notification.Applications.Services.Smtp;
using NoMoreTears.Modules.Notification.Applications.Services.Templates;
using NoMoreTears.Modules.Notification.Contracts.Emails;

namespace NoMoreTears.Modules.Notification.Providers;

public static class NotificationRegistrar
{
    /// <summary>
    ///     Root configuration section of the module.
    /// </summary>
    public const string SectionName = "Notification";

    /// <summary>
    ///     Registers SMTP email delivery with embedded templates; <see cref="SmtpOptions"/> are bound
    ///     from the <c>Smtp</c> sub-section and validated on application start.
    /// </summary>
    public static IServiceCollection AddEmailNotifications(
        this IServiceCollection services,
        IConfiguration section)
    {
        ArgumentNullException.ThrowIfNull(section);

        services
            .AddOptions<SmtpOptions>()
            .Bind(section.GetSection(SmtpOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<SmtpOptions>, SmtpOptionsValidator>());

        services.TryAddSingleton<IEmailTemplateStore, EmbeddedEmailTemplateStore>();
        services.TryAddSingleton<IEmailNotifier, SmtpEmailNotifier>();

        return services;
    }
}
