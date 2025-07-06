using System.ComponentModel.DataAnnotations;

namespace Web.UI.Models
{
    public class PromotionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = string.Empty; // Percentage, Amount
        public decimal MinOrderAmount { get; set; }
        public bool IsActive { get; set; }
        public string PromotionType { get; set; } = string.Empty; // Flash, Seasonal, Regular
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> ProductIds { get; set; } = new List<int>();
        public int MaxUses { get; set; }
        public int UsedCount { get; set; }

        // Computed properties
        public string FormattedDiscount => DiscountType == "Percentage" ? $"{DiscountAmount}%" : $"{DiscountAmount:N0} تومان";
        public string FormattedMinOrder => MinOrderAmount > 0 ? $"{MinOrderAmount:N0} تومان" : "بدون حداقل خرید";
        public string FormattedStartDate => StartDate.ToString("yyyy/MM/dd");
        public string FormattedEndDate => EndDate.ToString("yyyy/MM/dd");
        public bool IsExpired => DateTime.Now > EndDate;
        public bool IsUpcoming => DateTime.Now < StartDate;
        public bool IsRunning => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public TimeSpan TimeRemaining => EndDate - DateTime.Now;
        public string FormattedTimeRemaining => TimeRemaining.Days > 0 ? $"{TimeRemaining.Days} روز" : $"{TimeRemaining.Hours} ساعت";
        public double ProgressPercentage => Math.Min(100, (double)UsedCount / MaxUses * 100);
        public bool IsLimitedQuantity => MaxUses > 0;
        public int RemainingUses => Math.Max(0, MaxUses - UsedCount);
    }

    public class CouponViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = string.Empty; // Percentage, Amount
        public decimal MinOrderAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsUsed { get; set; }
        public int MaxUses { get; set; }
        public int UsedCount { get; set; }

        // Computed properties
        public string FormattedDiscount => DiscountType == "Percentage" ? $"{DiscountAmount}%" : $"{DiscountAmount:N0} تومان";
        public string FormattedMinOrder => MinOrderAmount > 0 ? $"{MinOrderAmount:N0} تومان" : "بدون حداقل خرید";
        public string FormattedExpirationDate => ExpirationDate.ToString("yyyy/MM/dd");
        public bool IsExpired => DateTime.Now > ExpirationDate;
        public bool IsValid => IsActive && !IsExpired && (MaxUses == 0 || UsedCount < MaxUses);
        public int RemainingUses => Math.Max(0, MaxUses - UsedCount);
    }

    public class PersonalizedOfferViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public string RecommendationReason { get; set; } = string.Empty;
        public List<int> ProductIds { get; set; } = new List<int>();

        // Computed properties
        public string FormattedDiscount => DiscountType == "Percentage" ? $"{DiscountAmount}%" : $"{DiscountAmount:N0} تومان";
        public string FormattedExpirationDate => ExpirationDate.ToString("yyyy/MM/dd");
        public bool IsExpired => DateTime.Now > ExpirationDate;
        public TimeSpan TimeRemaining => ExpirationDate - DateTime.Now;
        public string FormattedTimeRemaining => TimeRemaining.Days > 0 ? $"{TimeRemaining.Days} روز" : $"{TimeRemaining.Hours} ساعت";
    }

    public class FlashSaleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountPercentage { get; set; }
        public List<FlashSaleProductViewModel> Products { get; set; } = new List<FlashSaleProductViewModel>();
        public bool IsActive { get; set; }

        // Computed properties
        public string FormattedDiscount => $"{DiscountPercentage}%";
        public string FormattedStartDate => StartDate.ToString("yyyy/MM/dd HH:mm");
        public string FormattedEndDate => EndDate.ToString("yyyy/MM/dd HH:mm");
        public bool IsLive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public bool IsUpcoming => DateTime.Now < StartDate;
        public bool IsExpired => DateTime.Now > EndDate;
        public TimeSpan TimeRemaining => EndDate - DateTime.Now;
        public string FormattedTimeRemaining => $"{TimeRemaining.Days:D2}:{TimeRemaining.Hours:D2}:{TimeRemaining.Minutes:D2}:{TimeRemaining.Seconds:D2}";
        public double ProgressPercentage => Math.Min(100, (DateTime.Now - StartDate).TotalMinutes / (EndDate - StartDate).TotalMinutes * 100);
    }

    public class FlashSaleProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int OriginalStock { get; set; }
        public int RemainingStock { get; set; }
        public int MaxQuantityPerCustomer { get; set; }

        // Computed properties
        public string FormattedOriginalPrice => $"{OriginalPrice:N0} تومان";
        public string FormattedSalePrice => $"{SalePrice:N0} تومان";
        public decimal DiscountAmount => OriginalPrice - SalePrice;
        public string FormattedDiscountAmount => $"{DiscountAmount:N0} تومان";
        public decimal DiscountPercentage => Math.Round((DiscountAmount / OriginalPrice) * 100, 1);
        public string FormattedDiscountPercentage => $"{DiscountPercentage}%";
        public double StockPercentage => (double)RemainingStock / OriginalStock * 100;
        public bool IsLowStock => RemainingStock < OriginalStock * 0.2;
        public bool IsOutOfStock => RemainingStock <= 0;
        public string ProductUrl => $"/Product/Details/{ProductId}";
    }

    public class SeasonalOfferViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty; // Spring, Summer, Fall, Winter
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DiscountPercentage { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public bool IsActive { get; set; }

        // Computed properties
        public string FormattedDiscount => $"{DiscountPercentage}%";
        public string FormattedStartDate => StartDate.ToString("yyyy/MM/dd");
        public string FormattedEndDate => EndDate.ToString("yyyy/MM/dd");
        public bool IsLive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public bool IsUpcoming => DateTime.Now < StartDate;
        public bool IsExpired => DateTime.Now > EndDate;
        public TimeSpan TimeRemaining => EndDate - DateTime.Now;
        public string FormattedTimeRemaining => TimeRemaining.Days > 0 ? $"{TimeRemaining.Days} روز" : $"{TimeRemaining.Hours} ساعت";
        public string SeasonIcon => GetSeasonIcon(Season);
        public string SeasonClass => GetSeasonClass(Season);

        private string GetSeasonIcon(string season)
        {
            return season switch
            {
                "Spring" => "fas fa-seedling",
                "Summer" => "fas fa-sun",
                "Fall" => "fas fa-leaf",
                "Winter" => "fas fa-snowflake",
                _ => "fas fa-calendar"
            };
        }

        private string GetSeasonClass(string season)
        {
            return season switch
            {
                "Spring" => "season-spring",
                "Summer" => "season-summer",
                "Fall" => "season-fall",
                "Winter" => "season-winter",
                _ => "season-default"
            };
        }
    }

    public class BulkDiscountViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MinQuantity { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; } = string.Empty; // Percentage, Amount
        public List<int> ProductIds { get; set; } = new List<int>();
        public List<int> CategoryIds { get; set; } = new List<int>();

        // Computed properties
        public string FormattedDiscount => DiscountType == "Percentage" ? $"{DiscountAmount}%" : $"{DiscountAmount:N0} تومان";
        public string FormattedMinQuantity => $"{MinQuantity} عدد";
    }

    public class LoyaltyProgramViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserPoints { get; set; }
        public int PointsToNextLevel { get; set; }
        public string CurrentLevel { get; set; } = string.Empty;
        public string NextLevel { get; set; } = string.Empty;
        public decimal PointValue { get; set; } // How much each point is worth in currency
        public List<LoyaltyRewardViewModel> AvailableRewards { get; set; } = new List<LoyaltyRewardViewModel>();
        public List<LoyaltyTransactionViewModel> RecentTransactions { get; set; } = new List<LoyaltyTransactionViewModel>();

        // Computed properties
        public string FormattedUserPoints => $"{UserPoints:N0} امتیاز";
        public string FormattedPointsToNextLevel => $"{PointsToNextLevel:N0} امتیاز";
        public string FormattedPointValue => $"{PointValue:N0} تومان";
        public decimal TotalPointValue => UserPoints * PointValue;
        public string FormattedTotalPointValue => $"{TotalPointValue:N0} تومان";
        public double ProgressToNextLevel => PointsToNextLevel > 0 ? (double)UserPoints / (UserPoints + PointsToNextLevel) * 100 : 100;
    }

    public class LoyaltyRewardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RequiredPoints { get; set; }
        public decimal RewardValue { get; set; }
        public string RewardType { get; set; } = string.Empty; // Discount, Product, Service
        public bool IsAvailable { get; set; }

        // Computed properties
        public string FormattedRequiredPoints => $"{RequiredPoints:N0} امتیاز";
        public string FormattedRewardValue => $"{RewardValue:N0} تومان";
    }

    public class LoyaltyTransactionViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Points { get; set; }
        public string TransactionType { get; set; } = string.Empty; // Earned, Redeemed
        public DateTime TransactionDate { get; set; }

        // Computed properties
        public string FormattedPoints => $"{(TransactionType == "Earned" ? "+" : "-")}{Points:N0} امتیاز";
        public string FormattedTransactionDate => TransactionDate.ToString("yyyy/MM/dd");
        public string TransactionIcon => TransactionType == "Earned" ? "fas fa-plus-circle text-success" : "fas fa-minus-circle text-danger";
    }

    public class LoyaltyRedemptionViewModel
    {
        public int Id { get; set; }
        public int PointsRedeemed { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime RedemptionDate { get; set; }

        // Computed properties
        public string FormattedPointsRedeemed => $"{PointsRedeemed:N0} امتیاز";
        public string FormattedDiscountAmount => $"{DiscountAmount:N0} تومان";
        public string FormattedRedemptionDate => RedemptionDate.ToString("yyyy/MM/dd HH:mm");
    }

    public class ApplyCouponViewModel
    {
        [Required(ErrorMessage = "کد تخفیف الزامی است")]
        [StringLength(20, ErrorMessage = "کد تخفیف نباید بیشتر از 20 کاراکتر باشد")]
        public string CouponCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "مبلغ سفارش الزامی است")]
        [Range(0, double.MaxValue, ErrorMessage = "مبلغ سفارش باید بیشتر از صفر باشد")]
        public decimal OrderTotal { get; set; }
    }
}