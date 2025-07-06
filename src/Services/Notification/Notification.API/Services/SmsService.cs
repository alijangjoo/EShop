using System.Text;
using System.Text.Json;
using Notification.API.DTOs;

namespace Notification.API.Services;

public class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;
    private readonly HttpClient _httpClient;

    public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = new HttpClient();
    }

    public async Task<NotificationResult> SendSmsAsync(SmsRequest request)
    {
        try
        {
            var smsSettings = _configuration.GetSection("SmsSettings");
            var baseUrl = smsSettings["BaseUrl"];
            var username = smsSettings["Username"];
            var password = smsSettings["Password"];
            var from = request.From ?? smsSettings["From"];

            var payload = new
            {
                username = username,
                password = password,
                to = request.To,
                from = from,
                text = request.Message,
                isflash = false
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}SendSMS", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"SMS sent successfully to {request.To}");
                return new NotificationResult
                {
                    IsSuccess = true,
                    Message = "SMS sent successfully"
                };
            }
            else
            {
                _logger.LogError($"Failed to send SMS to {request.To}. Response: {responseContent}");
                return new NotificationResult
                {
                    IsSuccess = false,
                    Message = "Failed to send SMS",
                    ErrorMessage = responseContent
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send SMS to {request.To}");
            return new NotificationResult
            {
                IsSuccess = false,
                Message = "Failed to send SMS",
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<NotificationResult> SendOrderConfirmationSmsAsync(string phoneNumber, string orderNumber, decimal totalAmount)
    {
        var message = $"سفارش شما ثبت شد.\nشماره سفارش: {orderNumber}\nمبلغ: {totalAmount:N0} تومان\nتیم EShop";
        
        var request = new SmsRequest
        {
            To = phoneNumber,
            Message = message
        };

        return await SendSmsAsync(request);
    }

    public async Task<NotificationResult> SendPaymentConfirmationSmsAsync(string phoneNumber, string orderNumber, decimal amount)
    {
        var message = $"پرداخت موفق\nسفارش: {orderNumber}\nمبلغ: {amount:N0} تومان\nتیم EShop";
        
        var request = new SmsRequest
        {
            To = phoneNumber,
            Message = message
        };

        return await SendSmsAsync(request);
    }

    public async Task<NotificationResult> SendPaymentFailureSmsAsync(string phoneNumber, string orderNumber, string failureReason)
    {
        var message = $"خطا در پرداخت\nسفارش: {orderNumber}\nعلت: {failureReason}\nتیم EShop";
        
        var request = new SmsRequest
        {
            To = phoneNumber,
            Message = message
        };

        return await SendSmsAsync(request);
    }
}