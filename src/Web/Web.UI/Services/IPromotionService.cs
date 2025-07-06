using Web.UI.Models;

namespace Web.UI.Services
{
    public interface IPromotionService
    {
        Task<List<PromotionViewModel>> GetActivePromotionsAsync();
        Task<PromotionViewModel> GetPromotionByIdAsync(int id);
        Task<CouponViewModel> ValidateCouponAsync(string couponCode);
        Task<decimal> ApplyCouponAsync(string couponCode, decimal orderTotal);
        Task<List<PersonalizedOfferViewModel>> GetPersonalizedOffersAsync();
        Task SubscribeToOffersAsync(string email);
        Task<List<FlashSaleViewModel>> GetFlashSalesAsync();
        Task<List<SeasonalOfferViewModel>> GetSeasonalOffersAsync();
        Task<BulkDiscountViewModel> CheckBulkDiscountAsync(List<int> productIds, List<int> quantities);
        Task<LoyaltyProgramViewModel> GetLoyaltyProgramAsync();
        Task<LoyaltyRedemptionViewModel> RedeemLoyaltyPointsAsync(int points);
    }
}