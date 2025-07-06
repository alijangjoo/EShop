namespace Notification.API.DTOs;

public class SmsRequest
{
    public string To { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? From { get; set; }
}