namespace NoMoreTears.Modules.Notification.Applications.Services.Templates;

/// <summary>
///     Raw email template with <c>{{Key}}</c> placeholders.
/// </summary>
/// <param name="Subject">Subject line.</param>
/// <param name="HtmlBody">HTML body.</param>
/// <param name="TextBody">Plain-text alternative of the body.</param>
public sealed record EmailTemplate(
    string Subject,
    string HtmlBody,
    string TextBody);
