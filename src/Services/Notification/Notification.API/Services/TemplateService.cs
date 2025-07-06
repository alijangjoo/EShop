using System.Text;

namespace Notification.API.Services;

public class TemplateService : ITemplateService
{
    private readonly ILogger<TemplateService> _logger;
    private readonly Dictionary<string, string> _emailTemplates;
    private readonly Dictionary<string, string> _smsTemplates;

    public TemplateService(ILogger<TemplateService> logger)
    {
        _logger = logger;
        _emailTemplates = InitializeEmailTemplates();
        _smsTemplates = InitializeSmsTemplates();
    }

    public async Task<string> GetEmailTemplateAsync(string templateName, Dictionary<string, string> data)
    {
        if (_emailTemplates.TryGetValue(templateName, out var template))
        {
            return await Task.FromResult(ReplaceTokens(template, data));
        }
        
        _logger.LogWarning($"Email template '{templateName}' not found");
        return string.Empty;
    }

    public async Task<string> GetSmsTemplateAsync(string templateName, Dictionary<string, string> data)
    {
        if (_smsTemplates.TryGetValue(templateName, out var template))
        {
            return await Task.FromResult(ReplaceTokens(template, data));
        }
        
        _logger.LogWarning($"SMS template '{templateName}' not found");
        return string.Empty;
    }

    public string ReplaceTokens(string template, Dictionary<string, string> data)
    {
        var result = new StringBuilder(template);
        
        foreach (var kvp in data)
        {
            result.Replace($"{{{kvp.Key}}}", kvp.Value);
        }
        
        return result.ToString();
    }

    private Dictionary<string, string> InitializeEmailTemplates()
    {
        return new Dictionary<string, string>
        {
            ["OrderConfirmation"] = @"
                <div style='font-family: Tahoma, Arial, sans-serif; direction: rtl; text-align: right;'>
                    <h2 style='color: #2c3e50;'>تأیید سفارش شما</h2>
                    <p>با سلام {CustomerName} عزیز،</p>
                    <p>سفارش شما با موفقیت ثبت شد.</p>
                    <div style='background: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='color: #495057; margin-top: 0;'>جزئیات سفارش:</h3>
                        <p><strong>شماره سفارش:</strong> {OrderNumber}</p>
                        <p><strong>مبلغ کل:</strong> {TotalAmount} تومان</p>
                        <p><strong>روش پرداخت:</strong> {PaymentMethod}</p>
                        <p><strong>تاریخ ثبت:</strong> {OrderDate}</p>
                    </div>
                    <p>از خرید شما متشکریم.</p>
                    <p>با تشکر،<br/>تیم EShop</p>
                </div>",

            ["PaymentSuccess"] = @"
                <div style='font-family: Tahoma, Arial, sans-serif; direction: rtl; text-align: right;'>
                    <h2 style='color: #28a745;'>پرداخت موفق</h2>
                    <p>با سلام {CustomerName} عزیز،</p>
                    <p>پرداخت شما با موفقیت انجام شد.</p>
                    <div style='background: #d4edda; padding: 20px; border-radius: 5px; margin: 20px 0; border: 1px solid #c3e6cb;'>
                        <h3 style='color: #155724; margin-top: 0;'>جزئیات پرداخت:</h3>
                        <p><strong>شماره سفارش:</strong> {OrderNumber}</p>
                        <p><strong>مبلغ پرداخت شده:</strong> {Amount} تومان</p>
                        <p><strong>روش پرداخت:</strong> {PaymentMethod}</p>
                        <p><strong>تاریخ پرداخت:</strong> {PaymentDate}</p>
                    </div>
                    <p>سفارش شما در حال آماده‌سازی است و به زودی ارسال خواهد شد.</p>
                    <p>با تشکر،<br/>تیم EShop</p>
                </div>",

            ["Welcome"] = @"
                <div style='font-family: Tahoma, Arial, sans-serif; direction: rtl; text-align: right;'>
                    <h2 style='color: #2c3e50;'>به EShop خوش آمدید!</h2>
                    <p>با سلام {CustomerName} عزیز،</p>
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
                    <p>از اعتماد شما متشکریم!</p>
                    <p>با تشکر،<br/>تیم EShop</p>
                </div>"
        };
    }

    private Dictionary<string, string> InitializeSmsTemplates()
    {
        return new Dictionary<string, string>
        {
            ["OrderConfirmation"] = "سفارش شما ثبت شد.\nشماره سفارش: {OrderNumber}\nمبلغ: {TotalAmount} تومان\nتیم EShop",
            ["PaymentSuccess"] = "پرداخت موفق\nسفارش: {OrderNumber}\nمبلغ: {Amount} تومان\nتیم EShop",
            ["PaymentFailed"] = "خطا در پرداخت\nسفارش: {OrderNumber}\nعلت: {FailureReason}\nتیم EShop",
            ["Welcome"] = "سلام {CustomerName}!\nبه EShop خوش آمدید. از اعتماد شما متشکریم.\nتیم EShop"
        };
    }
}