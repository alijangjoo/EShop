namespace EventBus.Messages.Events;

public class NotificationEvent : IntegrationBaseEvent
{
    public string Type { get; set; } = string.Empty; // Email, SMS, Push
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ToName { get; set; }
    public Dictionary<string, string>? TemplateData { get; set; }
    public string? TemplateId { get; set; }
    public bool IsUrgent { get; set; } = false;
}