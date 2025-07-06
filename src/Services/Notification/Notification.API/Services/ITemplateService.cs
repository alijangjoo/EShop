namespace Notification.API.Services;

public interface ITemplateService
{
    Task<string> GetEmailTemplateAsync(string templateName, Dictionary<string, string> data);
    Task<string> GetSmsTemplateAsync(string templateName, Dictionary<string, string> data);
    string ReplaceTokens(string template, Dictionary<string, string> data);
}