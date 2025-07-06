namespace Admin.UI.Models;

// Dashboard Models
public class DashboardViewModel
{
    public DashboardStats Stats { get; set; } = new();
    public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
    public List<TopProductViewModel> TopProducts { get; set; } = new();
    public List<ChartDataViewModel> SalesChart { get; set; } = new();
}

public class DashboardStats
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int LowStockProducts { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
}

public class RecentOrderViewModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
}

public class TopProductViewModel
{
    public int Id { get; set; }
    public string NamePersian { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public int SoldQuantity { get; set; }
    public decimal Revenue { get; set; }
}

public class ChartDataViewModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Color { get; set; } = string.Empty;
}

// Product Management Models
public class ProductManagementViewModel
{
    public List<ProductViewModel> Products { get; set; } = new();
    public List<CategoryViewModel> Categories { get; set; } = new();
    public int TotalProducts { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public string? SortBy { get; set; }
}

public class ProductViewModel
{
    public int Id { get; set; }
    public string NamePersian { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public string DescriptionPersian { get; set; } = string.Empty;
    public string DescriptionEnglish { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsOnSale { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<ProductAttributeViewModel> Attributes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProductAttributeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CategoryViewModel
{
    public int Id { get; set; }
    public string NamePersian { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public string DescriptionPersian { get; set; } = string.Empty;
    public string DescriptionEnglish { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateProductViewModel
{
    public string NamePersian { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public string DescriptionPersian { get; set; } = string.Empty;
    public string DescriptionEnglish { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsOnSale { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    public List<ProductAttributeViewModel> Attributes { get; set; } = new();
}

public class EditProductViewModel : CreateProductViewModel
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Order Management Models
public class OrderManagementViewModel
{
    public List<OrderViewModel> Orders { get; set; } = new();
    public int TotalOrders { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class OrderViewModel
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<OrderItemViewModel> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderItemViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

// Customer Management Models
public class CustomerManagementViewModel
{
    public List<CustomerViewModel> Customers { get; set; } = new();
    public int TotalCustomers { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}

public class CustomerViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
}

// Reports Models
public class ReportViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public List<ReportDataViewModel> Data { get; set; } = new();
}

public class ReportDataViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal NumericValue { get; set; }
}

public class SalesReportViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<ChartDataViewModel> DailySales { get; set; } = new();
    public List<TopProductViewModel> TopProducts { get; set; } = new();
    public List<CategorySalesViewModel> CategorySales { get; set; } = new();
}

public class CategorySalesViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public int ProductCount { get; set; }
}

// Settings Models
public class SettingsViewModel
{
    public GeneralSettingsViewModel General { get; set; } = new();
    public PaymentSettingsViewModel Payment { get; set; } = new();
    public ShippingSettingsViewModel Shipping { get; set; } = new();
    public EmailSettingsViewModel Email { get; set; } = new();
}

public class GeneralSettingsViewModel
{
    public string SiteName { get; set; } = string.Empty;
    public string SiteDescription { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public bool MaintenanceMode { get; set; }
}

public class PaymentSettingsViewModel
{
    public bool EnableIPG { get; set; }
    public bool EnableCashOnDelivery { get; set; }
    public string IPGMerchantId { get; set; } = string.Empty;
    public string IPGApiKey { get; set; } = string.Empty;
    public string IPGCallbackUrl { get; set; } = string.Empty;
}

public class ShippingSettingsViewModel
{
    public bool EnableFreeShipping { get; set; }
    public decimal FreeShippingThreshold { get; set; }
    public decimal StandardShippingCost { get; set; }
    public decimal ExpressShippingCost { get; set; }
    public int StandardDeliveryDays { get; set; }
    public int ExpressDeliveryDays { get; set; }
}

public class EmailSettingsViewModel
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

// Authentication Models
public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class UserProfileViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

// Payment Management Models
public class PaymentManagementViewModel
{
    public List<PaymentViewModel> Payments { get; set; } = new();
    public int TotalPayments { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public string? SearchTerm { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

public class PaymentViewModel
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; }
    
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
}

public class PaymentStatsViewModel
{
    public int TotalPayments { get; set; }
    public int PendingPayments { get; set; }
    public int ProcessingPayments { get; set; }
    public int CompletedPayments { get; set; }
    public int FailedPayments { get; set; }
    public int CancelledPayments { get; set; }
    public int RefundedPayments { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TodayAmount { get; set; }
    public decimal ThisMonthAmount { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public int IPGPayments { get; set; }
    public int CashPayments { get; set; }
    public List<ChartDataViewModel> StatusChart { get; set; } = new();
    public List<ChartDataViewModel> MethodChart { get; set; } = new();
}

public class PaymentFilterViewModel
{
    public Guid? OrderId { get; set; }
    public string? UserName { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public PaymentStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "PaymentDate";
    public bool SortDescending { get; set; } = true;
}

public class UpdatePaymentStatusViewModel
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? BankName { get; set; }
    public string? BankNamePersian { get; set; }
    public string? FailureReason { get; set; }
    public string? FailureReasonPersian { get; set; }
    public string? Notes { get; set; }
    public string? NotesPersian { get; set; }
}

public class RefundPaymentViewModel
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string ReasonPersian { get; set; } = string.Empty;
}

// Payment Enums
public enum PaymentMethod
{
    IPG = 1,
    Cash = 2
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6
}