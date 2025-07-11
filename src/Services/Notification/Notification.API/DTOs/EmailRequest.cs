namespace Notification.API.DTOs;

public class EmailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string? ToName { get; set; }
    public List<string>? Cc { get; set; }
    public List<string>? Bcc { get; set; }
}