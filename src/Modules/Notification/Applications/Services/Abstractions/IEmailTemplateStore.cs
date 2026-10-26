using NoMoreTears.Modules.Notification.Applications.Services.Templates;

namespace NoMoreTears.Modules.Notification.Applications.Services.Abstractions;

/// <summary>
///     Source of email templates.
/// </summary>
public interface IEmailTemplateStore
{
    /// <summary>
    ///     Returns the template with the given name.
    /// </summary>
    /// <exception cref="InvalidOperationException">The template does not exist.</exception>
    ValueTask<EmailTemplate> GetAsync(
        string name,
        CancellationToken cancellationToken);
}
