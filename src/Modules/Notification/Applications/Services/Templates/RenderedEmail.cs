namespace NoMoreTears.Modules.Notification.Applications.Services.Templates;

/// <summary>
///     Email content with every placeholder substituted.
/// </summary>
/// <param name="Subject">Subject line.</param>
/// <param name="HtmlBody">HTML body.</param>
/// <param name="TextBody">Plain-text alternative of the body.</param>
public sealed record RenderedEmail(
    string Subject,
    string HtmlBody,
    string TextBody);
