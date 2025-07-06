using System.ComponentModel.DataAnnotations;

namespace Web.UI.Models
{
    public class OrderTrackingViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public string DeliveryCompany { get; set; } = string.Empty;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
        public List<TrackingUpdateViewModel> TrackingUpdates { get; set; } = new List<TrackingUpdateViewModel>();
        public bool CanCancel { get; set; }
        public bool CanReturn { get; set; }
        public bool IsDelivered { get; set; }
        public string Notes { get; set; } = string.Empty;

        // Computed properties
        public string FormattedOrderDate => OrderDate.ToString("yyyy/MM/dd HH:mm");
        public string FormattedTotalAmount => $"{TotalAmount:N0} تومان";
        public string FormattedEstimatedDeliveryDate => EstimatedDeliveryDate?.ToString("yyyy/MM/dd") ?? "نامشخص";
        public string FormattedActualDeliveryDate => ActualDeliveryDate?.ToString("yyyy/MM/dd HH:mm") ?? "-";
        public string StatusBadgeClass => GetStatusBadgeClass(Status);
        public string PaymentStatusBadgeClass => GetPaymentStatusBadgeClass(PaymentStatus);
        public int DeliveryProgress => GetDeliveryProgress(Status);
        public string CurrentStep => GetCurrentStep(Status);
        public bool IsInProgress => Status == "Processing" || Status == "Shipped";
        public TimeSpan OrderAge => DateTime.Now - OrderDate;
        public string FormattedOrderAge => OrderAge.Days > 0 ? $"{OrderAge.Days} روز پیش" : $"{OrderAge.Hours} ساعت پیش";

        private string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Pending" => "bg-warning",
                "Processing" => "bg-info",
                "Shipped" => "bg-primary",
                "Delivered" => "bg-success",
                "Cancelled" => "bg-danger",
                "Returned" => "bg-secondary",
                _ => "bg-light"
            };
        }

        private string GetPaymentStatusBadgeClass(string paymentStatus)
        {
            return paymentStatus switch
            {
                "Paid" => "bg-success",
                "Pending" => "bg-warning",
                "Failed" => "bg-danger",
                "Refunded" => "bg-info",
                _ => "bg-light"
            };
        }

        private int GetDeliveryProgress(string status)
        {
            return status switch
            {
                "Pending" => 25,
                "Processing" => 50,
                "Shipped" => 75,
                "Delivered" => 100,
                "Cancelled" => 0,
                "Returned" => 0,
                _ => 0
            };
        }

        private string GetCurrentStep(string status)
        {
            return status switch
            {
                "Pending" => "در انتظار تأیید",
                "Processing" => "در حال پردازش",
                "Shipped" => "ارسال شده",
                "Delivered" => "تحویل داده شده",
                "Cancelled" => "لغو شده",
                "Returned" => "مرجوع شده",
                _ => "نامشخص"
            };
        }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductUrl => $"/Product/Details/{ProductId}";
        public string FormattedUnitPrice => $"{UnitPrice:N0} تومان";
        public string FormattedTotalPrice => $"{TotalPrice:N0} تومان";
    }

    public class TrackingUpdateViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public bool IsImportant { get; set; }
        public string FormattedUpdateDate => UpdateDate.ToString("yyyy/MM/dd HH:mm");
        public string StatusIcon => GetStatusIcon(Status);
        public string StatusClass => GetStatusClass(Status);

        private string GetStatusIcon(string status)
        {
            return status switch
            {
                "OrderPlaced" => "fas fa-shopping-cart",
                "PaymentConfirmed" => "fas fa-credit-card",
                "Processing" => "fas fa-cogs",
                "Packaged" => "fas fa-box",
                "Shipped" => "fas fa-truck",
                "OutForDelivery" => "fas fa-shipping-fast",
                "Delivered" => "fas fa-check-circle",
                "Cancelled" => "fas fa-times-circle",
                "Returned" => "fas fa-undo",
                _ => "fas fa-info-circle"
            };
        }

        private string GetStatusClass(string status)
        {
            return status switch
            {
                "OrderPlaced" => "text-info",
                "PaymentConfirmed" => "text-success",
                "Processing" => "text-warning",
                "Packaged" => "text-primary",
                "Shipped" => "text-info",
                "OutForDelivery" => "text-warning",
                "Delivered" => "text-success",
                "Cancelled" => "text-danger",
                "Returned" => "text-secondary",
                _ => "text-muted"
            };
        }
    }

    public class TrackOrderViewModel
    {
        [Required(ErrorMessage = "شماره سفارش الزامی است")]
        [StringLength(50, ErrorMessage = "شماره سفارش نباید بیشتر از 50 کاراکتر باشد")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نمی‌باشد")]
        public string Email { get; set; } = string.Empty;
    }

    public class CancelOrderViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "دلیل لغو سفارش الزامی است")]
        [StringLength(500, ErrorMessage = "دلیل لغو نباید بیشتر از 500 کاراکتر باشد")]
        public string Reason { get; set; } = string.Empty;
    }

    public class OrderNotificationSubscriptionViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نمی‌باشد")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "شماره تلفن الزامی است")]
        [StringLength(15, ErrorMessage = "شماره تلفن نباید بیشتر از 15 کاراکتر باشد")]
        public string Phone { get; set; } = string.Empty;

        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = true;
    }
}