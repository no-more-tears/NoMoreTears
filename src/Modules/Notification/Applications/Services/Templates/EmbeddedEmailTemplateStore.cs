using System.Collections.Concurrent;
using System.Reflection;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;

namespace NoMoreTears.Modules.Notification.Applications.Services.Templates;

/// <summary>
///     Reads templates embedded into this assembly from <c>Templates/Emails/{Name}/</c>
///     (<c>subject.txt</c>, <c>body.html</c>, <c>body.txt</c>) and caches them for the process lifetime.
/// </summary>
public sealed class EmbeddedEmailTemplateStore : IEmailTemplateStore
{
    private static readonly Assembly Assembly = typeof(EmbeddedEmailTemplateStore).Assembly;
    private static readonly string ResourcePrefix = $"{Assembly.GetName().Name}.Templates.Emails";

    private readonly ConcurrentDictionary<string, EmailTemplate> _templates = new(StringComparer.Ordinal);

    public ValueTask<EmailTemplate> GetAsync(
        string name,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return ValueTask.FromResult(_templates.GetOrAdd(name, Load));
    }

    private static EmailTemplate Load(string name) =>
        new(
            ReadResource(name, "subject.txt").Trim(),
            ReadResource(name, "body.html"),
            ReadResource(name, "body.txt"));

    private static string ReadResource(
        string template,
        string file)
    {
        var resourceName = $"{ResourcePrefix}.{template}.{file}";

        using var stream = Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Email template '{template}' has no embedded '{file}'.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
