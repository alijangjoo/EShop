using System.ComponentModel.DataAnnotations;

namespace Web.UI.Models;

public class PaymentViewModel
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethodEnum PaymentMethod { get; set; }
    public PaymentStatusEnum Status { get; set; }
    
    // Customer Information
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Payment Details
    public string? CardName { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? BankName { get; set; }
    public string? BankNamePersian { get; set; }
    
    // Timestamps
    public DateTime PaymentDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Additional Information
    public string? Description { get; set; }
    public string? DescriptionPersian { get; set; }
    public string? Notes { get; set; }
    public string? NotesPersian { get; set; }
    public string? FailureReason { get; set; }
    public string? FailureReasonPersian { get; set; }
    
    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    
    // Display Properties
    public string StatusText => Status switch
    {
        PaymentStatusEnum.Pending => "در انتظار",
        PaymentStatusEnum.Processing => "در حال پردازش",
        PaymentStatusEnum.Completed => "تکمیل شده",
        PaymentStatusEnum.Failed => "ناموفق",
        PaymentStatusEnum.Cancelled => "لغو شده",
        PaymentStatusEnum.Refunded => "استرداد شده",
        _ => "نامشخص"
    };
    
    public string PaymentMethodText => PaymentMethod switch
    {
        PaymentMethodEnum.IPG => "درگاه پرداخت",
        PaymentMethodEnum.Cash => "نقدی",
        _ => "نامشخص"
    };
    
    public string StatusClass => Status switch
    {
        PaymentStatusEnum.Pending => "warning",
        PaymentStatusEnum.Processing => "info",
        PaymentStatusEnum.Completed => "success",
        PaymentStatusEnum.Failed => "danger",
        PaymentStatusEnum.Cancelled => "secondary",
        PaymentStatusEnum.Refunded => "dark",
        _ => "light"
    };
}

public class CreatePaymentViewModel
{
    public Guid OrderId { get; set; }
    
    [Required(ErrorMessage = "مبلغ پرداخت الزامی است")]
    [Range(0.01, double.MaxValue, ErrorMessage = "مبلغ باید بیشتر از صفر باشد")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "انتخاب روش پرداخت الزامی است")]
    public PaymentMethodEnum PaymentMethod { get; set; }
    
    // Customer Information
    [Required(ErrorMessage = "نام مشتری الزامی است")]
    [StringLength(100, ErrorMessage = "نام مشتری نباید بیشتر از 100 کاراکتر باشد")]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "ایمیل مشتری الزامی است")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
    [StringLength(200, ErrorMessage = "ایمیل نباید بیشتر از 200 کاراکتر باشد")]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "شماره تلفن مشتری الزامی است")]
    [Phone(ErrorMessage = "فرمت شماره تلفن صحیح نیست")]
    [StringLength(20, ErrorMessage = "شماره تلفن نباید بیشتر از 20 کاراکتر باشد")]
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Payment Details (for IPG)
    [StringLength(100, ErrorMessage = "نام کارت نباید بیشتر از 100 کاراکتر باشد")]
    public string? CardName { get; set; }
    
    [StringLength(19, ErrorMessage = "شماره کارت نباید بیشتر از 19 کاراکتر باشد")]
    public string? CardNumber { get; set; }
    
    [StringLength(5, ErrorMessage = "تاریخ انقضا نباید بیشتر از 5 کاراکتر باشد")]
    public string? Expiration { get; set; }
    
    [StringLength(4, ErrorMessage = "کد CVV نباید بیشتر از 4 کاراکتر باشد")]
    public string? CVV { get; set; }
    
    // Additional Information
    [StringLength(500, ErrorMessage = "توضیحات نباید بیشتر از 500 کاراکتر باشد")]
    public string? Description { get; set; }
    
    [StringLength(500, ErrorMessage = "توضیحات فارسی نباید بیشتر از 500 کاراکتر باشد")]
    public string? DescriptionPersian { get; set; }
    
    [StringLength(1000, ErrorMessage = "یادداشت نباید بیشتر از 1000 کاراکتر باشد")]
    public string? Notes { get; set; }
    
    [StringLength(1000, ErrorMessage = "یادداشت فارسی نباید بیشتر از 1000 کاراکتر باشد")]
    public string? NotesPersian { get; set; }
    
    // Order Information for Display
    public string? OrderNumber { get; set; }
    public List<OrderItemViewModel> OrderItems { get; set; } = new();
    public decimal OrderTotal { get; set; }
    
    // Validation Methods
    public bool IsIPGPayment => PaymentMethod == PaymentMethodEnum.IPG;
    public bool IsCashPayment => PaymentMethod == PaymentMethodEnum.Cash;
}



public class PaymentListViewModel
{
    public List<PaymentViewModel> Payments { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    // Filter Properties
    public PaymentFilterViewModel Filter { get; set; } = new();
    
    // Pagination Properties
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public int PreviousPage => Page - 1;
    public int NextPage => Page + 1;
}

public class PaymentFilterViewModel
{
    public Guid? OrderId { get; set; }
    public PaymentMethodEnum? PaymentMethod { get; set; }
    public PaymentStatusEnum? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "PaymentDate";
    public bool SortDescending { get; set; } = true;
}

public class PaymentResultViewModel
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MessagePersian { get; set; } = string.Empty;
    public PaymentViewModel? Payment { get; set; }
    public string? RedirectUrl { get; set; }
}

public class PaymentVerifyViewModel
{
    public string PaymentNumber { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MessagePersian { get; set; } = string.Empty;
    public PaymentViewModel? Payment { get; set; }
}

public enum PaymentMethodEnum
{
    IPG = 1,
    Cash = 2
}

public enum PaymentStatusEnum
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6
}