using EventBus.Messages.Events;
using MassTransit;
using Notification.API.Services;
using Notification.API.DTOs;

namespace Notification.API.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<UserRegisteredConsumer> _logger;

    public UserRegisteredConsumer(IEmailService emailService, ISmsService smsService, ILogger<UserRegisteredConsumer> logger)
    {
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var user = context.Message;
        
        _logger.LogInformation($"Processing welcome notification for user {user.UserName}");

        var customerName = $"{user.FirstName} {user.LastName}";

        // Send welcome email
        try
        {
            var htmlBody = $@"
                <div style='font-family: Tahoma, Arial, sans-serif; direction: rtl; text-align: right;'>
                    <h2 style='color: #2c3e50;'>به EShop خوش آمدید!</h2>
                    <p>با سلام {customerName} عزیز،</p>
                    <p>به خانواده بزرگ EShop خوش آمدید!</p>
                    <div style='background: #e8f5e8; padding: 20px; border-radius: 5px; margin: 20px 0; border: 1px solid #c3e6cb;'>
                        <h3 style='color: #155724; margin-top: 0;'>مزایای عضویت شما:</h3>
                        <ul style='padding-right: 20px;'>
                            <li>دسترسی به هزاران محصول با کیفیت</li>
                            <li>تخفیف‌های ویژه اعضا</li>
                            <li>ارسال رایگان برای سفارشات بالای 500 هزار تومان</li>
                            <li>پشتیبانی 24 ساعته</li>
                            <li>امکان پیگیری آسان سفارشات</li>
                        </ul>
                    </div>
                    <p>اطلاعات حساب کاربری شما:</p>
                    <div style='background: #f8f9fa; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                        <p><strong>نام کاربری:</strong> {user.UserName}</p>
                        <p><strong>ایمیل:</strong> {user.Email}</p>
                        <p><strong>تاریخ عضویت:</strong> {user.RegistrationDate:yyyy/MM/dd}</p>
                    </div>
                    <p>برای شروع خرید، به <a href='https://eshop.com'>وب‌سایت ما</a> مراجعه کنید.</p>
                    <p>از اعتماد شما متشکریم!</p>
                    <p>با تشکر،<br/>تیم EShop</p>
                </div>";

            var emailRequest = new EmailRequest
            {
                To = user.Email,
                ToName = customerName,
                Subject = "به EShop خوش آمدید! 🎉",
                Body = htmlBody,
                IsHtml = true
            };

            var emailResult = await _emailService.SendEmailAsync(emailRequest);

            if (emailResult.IsSuccess)
            {
                _logger.LogInformation($"Welcome email sent successfully to {user.Email}");
            }
            else
            {
                _logger.LogError($"Failed to send welcome email to {user.Email}: {emailResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while sending welcome email to {user.Email}");
        }

        // Send welcome SMS if phone number is provided
        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            try
            {
                var smsMessage = $"سلام {customerName}!\nبه EShop خوش آمدید. از اعتماد شما متشکریم.\nتیم EShop";
                
                var smsRequest = new SmsRequest
                {
                    To = user.PhoneNumber,
                    Message = smsMessage
                };

                var smsResult = await _smsService.SendSmsAsync(smsRequest);

                if (smsResult.IsSuccess)
                {
                    _logger.LogInformation($"Welcome SMS sent successfully to {user.PhoneNumber}");
                }
                else
                {
                    _logger.LogError($"Failed to send welcome SMS to {user.PhoneNumber}: {smsResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while sending welcome SMS to {user.PhoneNumber}");
            }
        }

        _logger.LogInformation($"Completed processing welcome notification for user {user.UserName}");
    }
}