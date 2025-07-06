using Microsoft.AspNetCore.Mvc;
using Web.UI.Models;
using Web.UI.Services;

namespace Web.UI.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IPromotionService _promotionService;

        public PromotionsController(IApiService apiService, IPromotionService promotionService)
        {
            _apiService = apiService;
            _promotionService = promotionService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var promotions = await _promotionService.GetActivePromotionsAsync();
                return View(promotions);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری تخفیفات: " + ex.Message;
                return View(new List<PromotionViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                if (promotion == null)
                {
                    return NotFound();
                }

                return View(promotion);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری جزئیات تخفیف: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidateCoupon(string couponCode)
        {
            try
            {
                var coupon = await _promotionService.ValidateCouponAsync(couponCode);
                if (coupon != null)
                {
                    return Json(new { 
                        success = true, 
                        message = "کد تخفیف معتبر است",
                        discount = coupon.DiscountAmount,
                        discountType = coupon.DiscountType,
                        description = coupon.Description
                    });
                }
                else
                {
                    return Json(new { success = false, message = "کد تخفیف نامعتبر است" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در بررسی کد تخفیف: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode, decimal orderTotal)
        {
            try
            {
                var discountAmount = await _promotionService.ApplyCouponAsync(couponCode, orderTotal);
                if (discountAmount > 0)
                {
                    return Json(new { 
                        success = true, 
                        message = "کد تخفیف با موفقیت اعمال شد",
                        discountAmount = discountAmount,
                        formattedDiscount = $"{discountAmount:N0} تومان"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "کد تخفیف قابل اعمال نیست" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در اعمال کد تخفیف: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonalizedOffers()
        {
            try
            {
                var offers = await _promotionService.GetPersonalizedOffersAsync();
                return Json(new { success = true, offers = offers });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeToOffers(string email)
        {
            try
            {
                await _promotionService.SubscribeToOffersAsync(email);
                return Json(new { success = true, message = "با موفقیت در خبرنامه تخفیفات عضو شدید" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> FlashSales()
        {
            try
            {
                var flashSales = await _promotionService.GetFlashSalesAsync();
                return View(flashSales);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری فروش ویژه: " + ex.Message;
                return View(new List<FlashSaleViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> SeasonalOffers()
        {
            try
            {
                var seasonalOffers = await _promotionService.GetSeasonalOffersAsync();
                return View(seasonalOffers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری پیشنهادات فصلی: " + ex.Message;
                return View(new List<SeasonalOfferViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckBulkDiscount(List<int> productIds, List<int> quantities)
        {
            try
            {
                var bulkDiscount = await _promotionService.CheckBulkDiscountAsync(productIds, quantities);
                if (bulkDiscount != null)
                {
                    return Json(new { 
                        success = true, 
                        discount = bulkDiscount.DiscountAmount,
                        description = bulkDiscount.Description
                    });
                }
                else
                {
                    return Json(new { success = false, message = "تخفیف عمده‌فروشی موجود نیست" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoyaltyProgram()
        {
            try
            {
                var loyaltyProgram = await _promotionService.GetLoyaltyProgramAsync();
                return View(loyaltyProgram);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری برنامه وفاداری: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RedeemLoyaltyPoints(int points)
        {
            try
            {
                var redemption = await _promotionService.RedeemLoyaltyPointsAsync(points);
                return Json(new { 
                    success = true, 
                    message = "امتیاز شما با موفقیت استفاده شد",
                    discountAmount = redemption.DiscountAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}