using Web.UI.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Web.UI.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PromotionService(IApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<PromotionViewModel>> GetActivePromotionsAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<List<PromotionViewModel>>("/api/promotion/active");
                return response ?? GenerateMockPromotions();
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error getting active promotions: {ex.Message}");
                return GenerateMockPromotions();
            }
        }

        public async Task<PromotionViewModel> GetPromotionByIdAsync(int id)
        {
            try
            {
                var response = await _apiService.GetAsync<PromotionViewModel>($"/api/promotion/{id}");
                return response ?? GenerateMockPromotions().FirstOrDefault(p => p.Id == id);
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error getting promotion by id: {ex.Message}");
                return GenerateMockPromotions().FirstOrDefault(p => p.Id == id);
            }
        }

        public async Task<CouponViewModel> ValidateCouponAsync(string couponCode)
        {
            try
            {
                var response = await _apiService.GetAsync<CouponViewModel>($"/api/promotion/coupon/validate?code={couponCode}");
                return response ?? GenerateMockCoupon(couponCode);
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error validating coupon: {ex.Message}");
                return GenerateMockCoupon(couponCode);
            }
        }

        public async Task<decimal> ApplyCouponAsync(string couponCode, decimal orderTotal)
        {
            try
            {
                var request = new { CouponCode = couponCode, OrderTotal = orderTotal };
                var response = await _apiService.PostAsync<decimal>("/api/promotion/coupon/apply", request);
                return response;
            }
            catch (Exception ex)
            {
                // Log error and return mock calculation
                Console.WriteLine($"Error applying coupon: {ex.Message}");
                return CalculateMockDiscount(couponCode, orderTotal);
            }
        }

        public async Task<List<PersonalizedOfferViewModel>> GetPersonalizedOffersAsync()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var response = await _apiService.GetAsync<List<PersonalizedOfferViewModel>>($"/api/promotion/personalized?userId={userId}");
                return response ?? GenerateMockPersonalizedOffers();
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error getting personalized offers: {ex.Message}");
                return GenerateMockPersonalizedOffers();
            }
        }

        public async Task SubscribeToOffersAsync(string email)
        {
            try
            {
                var request = new { Email = email };
                await _apiService.PostAsync("/api/promotion/subscribe", request);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error subscribing to offers: {ex.Message}");
                throw;
            }
        }

        public async Task<List<FlashSaleViewModel>> GetFlashSalesAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<List<FlashSaleViewModel>>("/api/promotion/flashsales");
                return response ?? GenerateMockFlashSales();
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error getting flash sales: {ex.Message}");
                return GenerateMockFlashSales();
            }
        }

        public async Task<List<SeasonalOfferViewModel>> GetSeasonalOffersAsync()
        {
            try
            {
                var response = await _apiService.GetAsync<List<SeasonalOfferViewModel>>("/api/promotion/seasonal");
                return response ?? GenerateMockSeasonalOffers();
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error getting seasonal offers: {ex.Message}");
                return GenerateMockSeasonalOffers();
            }
        }

        public async Task<BulkDiscountViewModel> CheckBulkDiscountAsync(List<int> productIds, List<int> quantities)
        {
            try
            {
                var request = new { ProductIds = productIds, Quantities = quantities };
                var response = await _apiService.PostAsync<BulkDiscountViewModel>("/api/promotion/bulk-discount", request);
                return response;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error checking bulk discount: {ex.Message}");
                return null;
            }
        }

        public async Task<LoyaltyProgramViewModel> GetLoyaltyProgramAsync()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var response = await _apiService.GetAsync<LoyaltyProgramViewModel>($"/api/promotion/loyalty?userId={userId}");
                return response ?? GenerateMockLoyaltyProgram();
            }
            catch (Exception ex)
            {
                // Log error and return mock data for demonstration
                Console.WriteLine($"Error getting loyalty program: {ex.Message}");
                return GenerateMockLoyaltyProgram();
            }
        }

        public async Task<LoyaltyRedemptionViewModel> RedeemLoyaltyPointsAsync(int points)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var request = new { UserId = userId, Points = points };
                var response = await _apiService.PostAsync<LoyaltyRedemptionViewModel>("/api/promotion/loyalty/redeem", request);
                return response;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error redeeming loyalty points: {ex.Message}");
                throw;
            }
        }

        // Mock data generation methods for demonstration
        private List<PromotionViewModel> GenerateMockPromotions()
        {
            return new List<PromotionViewModel>
            {
                new PromotionViewModel
                {
                    Id = 1,
                    Title = "تخفیف ویژه بهاری",
                    Description = "۳۰٪ تخفیف روی تمام محصولات پوشاک",
                    ImageUrl = "/images/promotions/spring-sale.jpg",
                    StartDate = DateTime.Now.AddDays(-7),
                    EndDate = DateTime.Now.AddDays(7),
                    DiscountAmount = 30,
                    DiscountType = "Percentage",
                    MinOrderAmount = 100000,
                    IsActive = true,
                    PromotionType = "Seasonal",
                    MaxUses = 1000,
                    UsedCount = 750
                },
                new PromotionViewModel
                {
                    Id = 2,
                    Title = "فروش فوری‌العاده",
                    Description = "۵۰٪ تخفیف روی محصولات منتخب",
                    ImageUrl = "/images/promotions/flash-sale.jpg",
                    StartDate = DateTime.Now.AddHours(-2),
                    EndDate = DateTime.Now.AddHours(22),
                    DiscountAmount = 50,
                    DiscountType = "Percentage",
                    MinOrderAmount = 0,
                    IsActive = true,
                    PromotionType = "Flash",
                    MaxUses = 500,
                    UsedCount = 350
                }
            };
        }

        private CouponViewModel GenerateMockCoupon(string couponCode)
        {
            if (couponCode.ToUpper() == "SAVE20")
            {
                return new CouponViewModel
                {
                    Id = 1,
                    Code = "SAVE20",
                    Description = "۲۰٪ تخفیف روی سفارش شما",
                    DiscountAmount = 20,
                    DiscountType = "Percentage",
                    MinOrderAmount = 50000,
                    ExpirationDate = DateTime.Now.AddDays(30),
                    IsActive = true,
                    IsUsed = false,
                    MaxUses = 100,
                    UsedCount = 25
                };
            }
            return null;
        }

        private decimal CalculateMockDiscount(string couponCode, decimal orderTotal)
        {
            if (couponCode.ToUpper() == "SAVE20" && orderTotal >= 50000)
            {
                return orderTotal * 0.2m;
            }
            return 0;
        }

        private List<PersonalizedOfferViewModel> GenerateMockPersonalizedOffers()
        {
            return new List<PersonalizedOfferViewModel>
            {
                new PersonalizedOfferViewModel
                {
                    Id = 1,
                    Title = "پیشنهاد ویژه برای شما",
                    Description = "۱۵٪ تخفیف روی محصولات مورد علاقه‌تان",
                    ImageUrl = "/images/offers/personalized-1.jpg",
                    DiscountAmount = 15,
                    DiscountType = "Percentage",
                    ExpirationDate = DateTime.Now.AddDays(7),
                    CouponCode = "PERSONAL15",
                    RecommendationReason = "بر اساس خریدهای قبلی شما"
                }
            };
        }

        private List<FlashSaleViewModel> GenerateMockFlashSales()
        {
            return new List<FlashSaleViewModel>
            {
                new FlashSaleViewModel
                {
                    Id = 1,
                    Title = "فروش فوری ۶ ساعته",
                    Description = "تخفیف‌های باورنکردنی فقط تا ۶ ساعت دیگر",
                    StartDate = DateTime.Now.AddHours(-2),
                    EndDate = DateTime.Now.AddHours(4),
                    DiscountPercentage = 60,
                    IsActive = true,
                    Products = new List<FlashSaleProductViewModel>
                    {
                        new FlashSaleProductViewModel
                        {
                            ProductId = 1,
                            ProductName = "لپ‌تاپ گیمینگ",
                            ProductImageUrl = "/images/products/laptop.jpg",
                            OriginalPrice = 25000000,
                            SalePrice = 10000000,
                            OriginalStock = 50,
                            RemainingStock = 12,
                            MaxQuantityPerCustomer = 1
                        }
                    }
                }
            };
        }

        private List<SeasonalOfferViewModel> GenerateMockSeasonalOffers()
        {
            return new List<SeasonalOfferViewModel>
            {
                new SeasonalOfferViewModel
                {
                    Id = 1,
                    Title = "فروش بهاری",
                    Description = "تخفیف‌های فوق‌العاده در فصل بهار",
                    ImageUrl = "/images/offers/spring.jpg",
                    Season = "Spring",
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(30),
                    DiscountPercentage = 25,
                    IsActive = true
                }
            };
        }

        private LoyaltyProgramViewModel GenerateMockLoyaltyProgram()
        {
            return new LoyaltyProgramViewModel
            {
                Id = 1,
                Title = "برنامه وفاداری طلایی",
                Description = "امتیاز جمع کنید و جوایز بگیرید",
                UserPoints = 2500,
                PointsToNextLevel = 500,
                CurrentLevel = "نقره‌ای",
                NextLevel = "طلایی",
                PointValue = 10,
                AvailableRewards = new List<LoyaltyRewardViewModel>
                {
                    new LoyaltyRewardViewModel
                    {
                        Id = 1,
                        Title = "تخفیف ۱۰۰ هزار تومانی",
                        Description = "کوپن تخفیف برای خرید بعدی",
                        RequiredPoints = 1000,
                        RewardValue = 100000,
                        RewardType = "Discount",
                        IsAvailable = true
                    }
                },
                RecentTransactions = new List<LoyaltyTransactionViewModel>
                {
                    new LoyaltyTransactionViewModel
                    {
                        Id = 1,
                        Description = "خرید محصولات",
                        Points = 250,
                        TransactionType = "Earned",
                        TransactionDate = DateTime.Now.AddDays(-5)
                    }
                }
            };
        }
    }
}