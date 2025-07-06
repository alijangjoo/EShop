namespace EventBus.Messages.Events;

public class UserRegisteredEvent : IntegrationBaseEvent
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
}