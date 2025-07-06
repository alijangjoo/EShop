using MailKit.Net.Smtp;
using MimeKit;
using Notification.API.DTOs;

namespace Notification.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<NotificationResult> SendEmailAsync(EmailRequest request)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress(emailSettings["FromName"], emailSettings["FromEmail"]));
            message.To.Add(new MailboxAddress(request.ToName ?? "", request.To));
            message.Subject = request.Subject;

            // Add CC recipients if provided
            if (request.Cc != null && request.Cc.Any())
            {
                foreach (var cc in request.Cc)
                {
                    message.Cc.Add(new MailboxAddress("", cc));
                }
            }

            // Add BCC recipients if provided
            if (request.Bcc != null && request.Bcc.Any())
            {
                foreach (var bcc in request.Bcc)
                {
                    message.Bcc.Add(new MailboxAddress("", bcc));
                }
            }

            var bodyBuilder = new BodyBuilder();
            if (request.IsHtml)
            {
                bodyBuilder.HtmlBody = request.Body;
            }
            else
            {
                bodyBuilder.TextBody = request.Body;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]!), true);
            await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Email sent successfully to {request.To}");
            return new NotificationResult
            {
                IsSuccess = true,
                Message = "Email sent successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {request.To}");
            return new NotificationResult
            {
                IsSuccess = false,
                Message = "Failed to send email",
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<NotificationResult> SendOrderConfirmationEmailAsync(string toEmail, string toName, string orderNumber, decimal totalAmount, string paymentMethod)
    {
        var paymentMethodPersian = paymentMethod switch
        {
            "IPG" => "درگاه پرداخت آنلاین",
            "Cash" => "پرداخت نقدی هنگام تحویل",
            _ => paymentMethod
        };

        var htmlBody = $@"
            <div style='font-family: Tahoma, Arial, sans-serif; direction: rtl; text-align: right;'>
                <h2 style='color: #2c3e50;'>تأیید سفارش شما</h2>
                <p>با سلام {toName} عزیز،</p>
                <p>سفارش شما با موفقیت ثبت شد.</p>
                <div style='background: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <h3 style='color: #495057; margin-top: 0;'>جزئیات سفارش:</h3>
                    <p><strong>شماره سفارش:</strong> {orderNumber}</p>
                    <p><strong>مبلغ کل:</strong> {totalAmount:N0} تومان</p>
                    <p><strong>روش پرداخت:</strong> {paymentMethodPersian}</p>
                    <p><strong>تاریخ ثبت:</strong> {DateTime.Now:yyyy/MM/dd HH:mm}</p>
                </div>
                <p>از خرید شما متشکریم.</p>
                <p>با تشکر،<br/>تیم EShop</p>
            </div>";

        var request = new EmailRequest
        {
            To = toEmail,
            ToName = toName,
            Subject = $"تأیید سفارش #{orderNumber} - EShop",
            Body = htmlBody,
            IsHtml = true
        };

        return await SendEmailAsync(request);
    }

    public async Task<NotificationResult> SendPaymentConfirmationEmailAsync(string toEmail, string toName, string orderNumber, decimal amount, string paymentMethod)
    {
        var paymentMethodPersian = paymentMethod switch
        {
            "IPG" => "درگاه پرداخت آنلاین",
            "Cash" => "پرداخت نقدی هنگام تحویل",
            _ => paymentMethod
        };

        var htmlBody = $@"
            <div style='font-family: Tahoma, Arial, sans-serif; direction: rtl; text-align: right;'>
                <h2 style='color: #28a745;'>پرداخت موفق</h2>
                <p>با سلام {toName} عزیز،</p>
                <p>پرداخت شما با موفقیت انجام شد.</p>
                <div style='background: #d4edda; padding: 20px; border-radius: 5px; margin: 20px 0; border: 1px solid #c3e6cb;'>
                    <h3 style='color: #155724; margin-top: 0;'>جزئیات پرداخت:</h3>
                    <p><strong>شماره سفارش:</strong> {orderNumber}</p>
                    <p><strong>مبلغ پرداخت شده:</strong> {amount:N0} تومان</p>
                    <p><strong>روش پرداخت:</strong> {paymentMethodPersian}</p>
                    <p><strong>تاریخ پرداخت:</strong> {DateTime.Now:yyyy/MM/dd HH:mm}</p>
                </div>
                <p>سفارش شما در حال آماده‌سازی است و به زودی ارسال خواهد شد.</p>
                <p>با تشکر،<br/>تیم EShop</p>
            </div>";

        var request = new EmailRequest
        {
            To = toEmail,
            ToName = toName,
            Subject = $"پرداخت موفق سفارش #{orderNumber} - EShop",
            Body = htmlBody,
            IsHtml = true
        };

        return await SendEmailAsync(request);
    }

    public async Task<NotificationResult> SendPaymentFailureEmailAsync(string toEmail, string toName, string orderNumber, decimal amount, string failureReason)
    {
        var htmlBody = $@"
            <div style='font-family: Tahoma, Arial, sans-serif; direction: rtl; text-align: right;'>
                <h2 style='color: #dc3545;'>خطا در پرداخت</h2>
                <p>با سلام {toName} عزیز،</p>
                <p>متأسفانه پرداخت شما ناموفق بود.</p>
                <div style='background: #f8d7da; padding: 20px; border-radius: 5px; margin: 20px 0; border: 1px solid #f1aeb5;'>
                    <h3 style='color: #721c24; margin-top: 0;'>جزئیات:</h3>
                    <p><strong>شماره سفارش:</strong> {orderNumber}</p>
                    <p><strong>مبلغ:</strong> {amount:N0} تومان</p>
                    <p><strong>علت خطا:</strong> {failureReason}</p>
                    <p><strong>تاریخ:</strong> {DateTime.Now:yyyy/MM/dd HH:mm}</p>
                </div>
                <p>لطفاً مجدداً تلاش کنید یا با پشتیبانی تماس بگیرید.</p>
                <p>با تشکر،<br/>تیم EShop</p>
            </div>";

        var request = new EmailRequest
        {
            To = toEmail,
            ToName = toName,
            Subject = $"خطا در پرداخت سفارش #{orderNumber} - EShop",
            Body = htmlBody,
            IsHtml = true
        };

        return await SendEmailAsync(request);
    }
}