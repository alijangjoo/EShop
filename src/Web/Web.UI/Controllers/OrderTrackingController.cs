using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web.UI.Models;
using Web.UI.Services;

namespace Web.UI.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly IApiService _apiService;
        private readonly IOrderTrackingService _orderTrackingService;

        public OrderTrackingController(IApiService apiService, IOrderTrackingService orderTrackingService)
        {
            _apiService = apiService;
            _orderTrackingService = orderTrackingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Track(string orderNumber, string email)
        {
            try
            {
                var orderTracking = await _orderTrackingService.GetOrderTrackingAsync(orderNumber, email);
                
                if (orderTracking == null)
                {
                    ViewBag.Error = "سفارش یافت نشد. لطفاً شماره سفارش و ایمیل خود را بررسی کنید.";
                    return View("Index");
                }

                return View("TrackingDetails", orderTracking);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری اطلاعات سفارش: " + ex.Message;
                return View("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int orderId)
        {
            try
            {
                var orderTracking = await _orderTrackingService.GetOrderTrackingByIdAsync(orderId);
                
                if (orderTracking == null)
                {
                    return NotFound();
                }

                return View("TrackingDetails", orderTracking);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری اطلاعات سفارش: " + ex.Message;
                return View("Index");
            }
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            try
            {
                var orders = await _orderTrackingService.GetUserOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری لیست سفارشات: " + ex.Message;
                return View(new List<OrderTrackingViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTrackingUpdates(int orderId)
        {
            try
            {
                var updates = await _orderTrackingService.GetTrackingUpdatesAsync(orderId);
                return Json(new { success = true, updates = updates });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RequestDeliveryUpdate(int orderId, string customerPhone)
        {
            try
            {
                await _orderTrackingService.RequestDeliveryUpdateAsync(orderId, customerPhone);
                return Json(new { success = true, message = "درخواست بروزرسانی ارسال شد. به زودی با شما تماس خواهیم گرفت." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId, string reason)
        {
            try
            {
                await _orderTrackingService.CancelOrderAsync(orderId, reason);
                return Json(new { success = true, message = "درخواست لغو سفارش ثبت شد." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintInvoice(int orderId)
        {
            try
            {
                var orderTracking = await _orderTrackingService.GetOrderTrackingByIdAsync(orderId);
                
                if (orderTracking == null)
                {
                    return NotFound();
                }

                return View("PrintInvoice", orderTracking);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "خطا در بارگیری فاکتور: " + ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubscribeToNotifications(int orderId, string email, string phone)
        {
            try
            {
                await _orderTrackingService.SubscribeToNotificationsAsync(orderId, email, phone);
                return Json(new { success = true, message = "اشتراک اطلاع‌رسانی فعال شد." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}