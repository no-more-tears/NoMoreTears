namespace NoMoreTears.Modules.Notification.Contracts.Emails;

/// <summary>
///     A templated email addressed to a single recipient.
/// </summary>
/// <param name="To">Recipient email address.</param>
/// <param name="Template">Name of the template, one of <see cref="EmailTemplates"/>.</param>
/// <param name="Model">Values substituted for the <c>{{Key}}</c> placeholders of the template.</param>
public sealed record EmailNotification(
    string To,
    string Template,
    IReadOnlyDictionary<string, string> Model);