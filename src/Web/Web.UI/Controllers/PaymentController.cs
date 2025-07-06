using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Web.UI.Models;

namespace Web.UI.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentController> _logger;
    private readonly IConfiguration _configuration;

    public PaymentController(IHttpClientFactory httpClientFactory, ILogger<PaymentController> logger, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _configuration = configuration;
        
        // Set base address for Payment API
        _httpClient.BaseAddress = new Uri(_configuration["Services:PaymentAPI"] ?? "https://localhost:5004");
    }

    // GET: Payment
    public async Task<IActionResult> Index(PaymentFilterViewModel filter)
    {
        try
        {
            var token = GetUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var queryString = BuildQueryString(filter);
            var response = await _httpClient.GetAsync($"/api/payment{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var paymentsResult = JsonSerializer.Deserialize<PaginatedPaymentApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var viewModel = new PaymentListViewModel
                {
                    Payments = paymentsResult?.Payments?.Select(MapFromApi).ToList() ?? new List<PaymentViewModel>(),
                    TotalCount = paymentsResult?.TotalCount ?? 0,
                    Page = paymentsResult?.Page ?? 1,
                    PageSize = paymentsResult?.PageSize ?? 10,
                    Filter = filter
                };

                return View(viewModel);
            }
            else
            {
                _logger.LogError("Error fetching payments: {StatusCode}", response.StatusCode);
                return View(new PaymentListViewModel());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Payment Index");
            return View(new PaymentListViewModel());
        }
    }

    // GET: Payment/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var token = GetUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/api/payment/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var payment = JsonSerializer.Deserialize<PaymentApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (payment != null)
                {
                    var viewModel = MapFromApi(payment);
                    return View(viewModel);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Payment Details");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Payment/Create
    public async Task<IActionResult> Create(Guid orderId)
    {
        try
        {
            // Get order details to populate payment form
            var orderDetails = await GetOrderDetails(orderId);
            
            if (orderDetails == null)
            {
                TempData["Error"] = "سفارش یافت نشد";
                return RedirectToAction("Index", "Order");
            }

            var viewModel = new CreatePaymentViewModel
            {
                OrderId = orderId,
                Amount = orderDetails.Total,
                OrderNumber = orderDetails.OrderNumber,
                OrderItems = orderDetails.Items,
                OrderTotal = orderDetails.Total,
                CustomerName = orderDetails.CustomerName,
                CustomerEmail = orderDetails.CustomerEmail,
                CustomerPhone = orderDetails.CustomerPhone
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Payment Create GET");
            TempData["Error"] = "خطا در بارگذاری صفحه پرداخت";
            return RedirectToAction("Index", "Order");
        }
    }

    // POST: Payment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePaymentViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                // Reload order details if validation fails
                var orderDetails = await GetOrderDetails(model.OrderId);
                if (orderDetails != null)
                {
                    model.OrderNumber = orderDetails.OrderNumber;
                    model.OrderItems = orderDetails.Items;
                    model.OrderTotal = orderDetails.Total;
                }
                return View(model);
            }

            var token = GetUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createPaymentDto = new
            {
                OrderId = model.OrderId,
                Amount = model.Amount,
                PaymentMethod = (int)model.PaymentMethod,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CardName = model.CardName,
                CardNumber = model.CardNumber,
                Expiration = model.Expiration,
                CVV = model.CVV,
                Description = model.Description,
                DescriptionPersian = model.DescriptionPersian,
                Notes = model.Notes,
                NotesPersian = model.NotesPersian
            };

            var json = JsonSerializer.Serialize(createPaymentDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/payment", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PaymentResultApiResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null && result.IsSuccess)
                {
                    if (!string.IsNullOrEmpty(result.RedirectUrl))
                    {
                        // Redirect to payment gateway for IPG payments
                        return Redirect(result.RedirectUrl);
                    }
                    else
                    {
                        // Success for cash payments
                        TempData["Success"] = result.MessagePersian ?? result.Message;
                        return RedirectToAction(nameof(Details), new { id = result.Payment?.Id });
                    }
                }
                else
                {
                    TempData["Error"] = result?.MessagePersian ?? result?.Message ?? "خطا در پردازش پرداخت";
                }
            }
            else
            {
                TempData["Error"] = "خطا در ارتباط با سرور پرداخت";
            }

            // Reload order details if error occurs
            var orderDetailsOnError = await GetOrderDetails(model.OrderId);
            if (orderDetailsOnError != null)
            {
                model.OrderNumber = orderDetailsOnError.OrderNumber;
                model.OrderItems = orderDetailsOnError.Items;
                model.OrderTotal = orderDetailsOnError.Total;
            }

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Payment Create POST");
            TempData["Error"] = "خطا در پردازش پرداخت";
            return View(model);
        }
    }

    // GET: Payment/Verify
    [AllowAnonymous]
    public async Task<IActionResult> Verify(string paymentNumber, string transactionId)
    {
        try
        {
            if (string.IsNullOrEmpty(paymentNumber) || string.IsNullOrEmpty(transactionId))
            {
                return BadRequest("پارامترهای تایید پرداخت نامعتبر است");
            }

            var verifyDto = new
            {
                PaymentNumber = paymentNumber,
                TransactionId = transactionId
            };

            var json = JsonSerializer.Serialize(verifyDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/payment/verify", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PaymentResultApiResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var viewModel = new PaymentVerifyViewModel
                {
                    PaymentNumber = paymentNumber,
                    TransactionId = transactionId,
                    IsSuccess = result?.IsSuccess ?? false,
                    Message = result?.Message ?? string.Empty,
                    MessagePersian = result?.MessagePersian ?? string.Empty,
                    Payment = result?.Payment != null ? MapFromApi(result.Payment) : null
                };

                return View(viewModel);
            }
            else
            {
                var viewModel = new PaymentVerifyViewModel
                {
                    PaymentNumber = paymentNumber,
                    TransactionId = transactionId,
                    IsSuccess = false,
                    Message = "Payment verification failed",
                    MessagePersian = "تایید پرداخت ناموفق بود"
                };

                return View(viewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Payment Verify");
            var viewModel = new PaymentVerifyViewModel
            {
                PaymentNumber = paymentNumber,
                TransactionId = transactionId,
                IsSuccess = false,
                Message = "Internal server error",
                MessagePersian = "خطای داخلی سرور"
            };

            return View(viewModel);
        }
    }

    // GET: Payment/MyPayments
    public async Task<IActionResult> MyPayments()
    {
        try
        {
            var token = GetUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/api/payment/user");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var payments = JsonSerializer.Deserialize<List<PaymentApiResponse>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var viewModel = payments?.Select(MapFromApi).ToList() ?? new List<PaymentViewModel>();
                return View(viewModel);
            }
            else
            {
                _logger.LogError("Error fetching user payments: {StatusCode}", response.StatusCode);
                return View(new List<PaymentViewModel>());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MyPayments");
            return View(new List<PaymentViewModel>());
        }
    }

    // Helper Methods
    private string GetUserToken()
    {
        // Extract JWT token from user claims or session
        // This is a simplified implementation
        return HttpContext.Session.GetString("JwtToken") ?? string.Empty;
    }

    private string BuildQueryString(PaymentFilterViewModel filter)
    {
        var queryParams = new List<string>();

        if (filter.OrderId.HasValue)
            queryParams.Add($"OrderId={filter.OrderId.Value}");

        if (filter.PaymentMethod.HasValue)
            queryParams.Add($"PaymentMethod={filter.PaymentMethod.Value}");

        if (filter.Status.HasValue)
            queryParams.Add($"Status={filter.Status.Value}");

        if (filter.FromDate.HasValue)
            queryParams.Add($"FromDate={filter.FromDate.Value:yyyy-MM-dd}");

        if (filter.ToDate.HasValue)
            queryParams.Add($"ToDate={filter.ToDate.Value:yyyy-MM-dd}");

        if (filter.MinAmount.HasValue)
            queryParams.Add($"MinAmount={filter.MinAmount.Value}");

        if (filter.MaxAmount.HasValue)
            queryParams.Add($"MaxAmount={filter.MaxAmount.Value}");

        if (!string.IsNullOrEmpty(filter.SearchTerm))
            queryParams.Add($"SearchTerm={Uri.EscapeDataString(filter.SearchTerm)}");

        queryParams.Add($"Page={filter.Page}");
        queryParams.Add($"PageSize={filter.PageSize}");

        if (!string.IsNullOrEmpty(filter.SortBy))
            queryParams.Add($"SortBy={filter.SortBy}");

        queryParams.Add($"SortDescending={filter.SortDescending}");

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }

    private PaymentViewModel MapFromApi(PaymentApiResponse apiResponse)
    {
        return new PaymentViewModel
        {
            Id = apiResponse.Id,
            PaymentNumber = apiResponse.PaymentNumber,
            OrderId = apiResponse.OrderId,
            UserName = apiResponse.UserName,
            Amount = apiResponse.Amount,
            PaymentMethod = (PaymentMethodEnum)apiResponse.PaymentMethod,
            Status = (PaymentStatusEnum)apiResponse.Status,
            CustomerName = apiResponse.CustomerName,
            CustomerEmail = apiResponse.CustomerEmail,
            CustomerPhone = apiResponse.CustomerPhone,
            CardName = apiResponse.CardName,
            CardLastFourDigits = apiResponse.CardLastFourDigits,
            TransactionId = apiResponse.TransactionId,
            ReferenceNumber = apiResponse.ReferenceNumber,
            GatewayTransactionId = apiResponse.GatewayTransactionId,
            BankName = apiResponse.BankName,
            BankNamePersian = apiResponse.BankNamePersian,
            PaymentDate = apiResponse.PaymentDate,
            ProcessedDate = apiResponse.ProcessedDate,
            CompletedDate = apiResponse.CompletedDate,
            Description = apiResponse.Description,
            DescriptionPersian = apiResponse.DescriptionPersian,
            Notes = apiResponse.Notes,
            NotesPersian = apiResponse.NotesPersian,
            FailureReason = apiResponse.FailureReason,
            FailureReasonPersian = apiResponse.FailureReasonPersian,
            CreatedAt = apiResponse.CreatedAt,
            UpdatedAt = apiResponse.UpdatedAt,
            CreatedBy = apiResponse.CreatedBy,
            UpdatedBy = apiResponse.UpdatedBy
        };
    }

    private async Task<OrderDetailsViewModel?> GetOrderDetails(Guid orderId)
    {
        try
        {
            var token = GetUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Call Order API to get order details
            var orderApiUrl = _configuration["Services:OrderAPI"] ?? "https://localhost:5003";
            var response = await _httpClient.GetAsync($"{orderApiUrl}/api/order/{orderId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var order = JsonSerializer.Deserialize<OrderApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (order != null)
                {
                    return new OrderDetailsViewModel
                    {
                        OrderId = order.Id,
                        OrderNumber = order.OrderNumber,
                        Total = order.TotalPrice,
                        CustomerName = order.CustomerName,
                        CustomerEmail = order.EmailAddress,
                        CustomerPhone = order.PhoneNumber,
                        Items = order.OrderItems?.Select(item => new OrderItemViewModel
                        {
                            ProductId = item.ProductId.GetHashCode(), // Convert Guid to int
                            ProductName = item.ProductName,
                            ProductNamePersian = item.ProductNamePersian,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            ImageUrl = item.ImageUrl
                        }).ToList() ?? new List<OrderItemViewModel>()
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order details for {OrderId}", orderId);
            return null;
        }
    }
}

// API Response Models
public class PaymentApiResponse
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; }
    public int Status { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CardName { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? TransactionId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? BankName { get; set; }
    public string? BankNamePersian { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Description { get; set; }
    public string? DescriptionPersian { get; set; }
    public string? Notes { get; set; }
    public string? NotesPersian { get; set; }
    public string? FailureReason { get; set; }
    public string? FailureReasonPersian { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}

public class PaginatedPaymentApiResponse
{
    public List<PaymentApiResponse> Payments { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class PaymentResultApiResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string MessagePersian { get; set; } = string.Empty;
    public PaymentApiResponse? Payment { get; set; }
    public string? RedirectUrl { get; set; }
}

public class OrderDetailsViewModel
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public List<OrderItemViewModel> Items { get; set; } = new();
}

public class OrderApiResponse
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<OrderItemApiResponse>? OrderItems { get; set; }
}

public class OrderItemApiResponse
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductNamePersian { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}