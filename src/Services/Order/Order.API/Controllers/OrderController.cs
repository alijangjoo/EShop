using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Order.API.Data;
using Order.API.DTOs;
using Order.API.Entities;
using MassTransit;
using EventBus.Messages.Events;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderController> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderController(
        OrderDbContext context, 
        IMapper mapper, 
        ILogger<OrderController> logger,
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? userName = null,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] PaymentStatus? paymentStatus = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(o => o.UserName == userName);
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (paymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= toDate.Value);
            }

            // Apply user-specific filtering for non-admin users
            if (!User.IsInRole("Admin"))
            {
                var currentUserName = User.Identity?.Name;
                if (string.IsNullOrEmpty(currentUserName))
                {
                    return Unauthorized();
                }
                query = query.Where(o => o.UserName == currentUserName);
            }

            var totalItems = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderDto>>(orders);

            Response.Headers.Add("X-Total-Count", totalItems.ToString());
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());

            return Ok(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching orders");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        try
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Id == id);

            // Apply user-specific filtering for non-admin users
            if (!User.IsInRole("Admin"))
            {
                var currentUserName = User.Identity?.Name;
                if (string.IsNullOrEmpty(currentUserName))
                {
                    return Unauthorized();
                }
                query = query.Where(o => o.UserName == currentUserName);
            }

            var order = await query.FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching order with ID: {OrderId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _mapper.Map<Entities.Order>(createOrderDto);
            order.Id = Guid.NewGuid();
            order.OrderNumber = GenerateOrderNumber();
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;
            order.PaymentStatus = PaymentStatus.Pending;
            order.CreatedBy = User.Identity?.Name ?? "System";
            order.UpdatedBy = User.Identity?.Name ?? "System";

            // Calculate totals
            order.TotalAmount = order.OrderItems.Sum(item => item.TotalPrice);
            order.ShippingCost = CalculateShippingCost(order);
            order.TaxAmount = CalculateTax(order);

            foreach (var item in order.OrderItems)
            {
                item.Id = Guid.NewGuid();
                item.TotalPrice = item.UnitPrice * item.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Publish event for other services
            var orderCheckoutEvent = new OrderCheckoutEvent
            {
                UserName = order.UserName,
                TotalPrice = order.TotalAmount,
                FirstName = order.FirstName,
                LastName = order.LastName,
                EmailAddress = order.Email,
                PhoneNumber = order.Phone,
                AddressLine = order.ShippingAddress,
                City = order.ShippingCity,
                State = order.ShippingState,
                Country = order.ShippingCountry,
                ZipCode = order.ShippingZipCode,
                CardName = order.CardName ?? "",
                CardNumber = order.CardNumber ?? "",
                Expiration = order.CardExpiration ?? "",
                CVV = order.CVV ?? "",
                PaymentMethod = (int)order.PaymentMethod
            };

            await _publishEndpoint.Publish(orderCheckoutEvent);

            var orderDto = _mapper.Map<OrderDto>(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = updateStatusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = User.Identity?.Name ?? "System";

            if (updateStatusDto.Status == OrderStatus.Shipped)
            {
                order.ShippedDate = DateTime.UtcNow;
                order.TrackingNumber = updateStatusDto.TrackingNumber;
            }
            else if (updateStatusDto.Status == OrderStatus.Delivered)
            {
                order.DeliveredDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating order status for ID: {OrderId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/payment-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] UpdatePaymentStatusDto updatePaymentStatusDto)
    {
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.PaymentStatus = updatePaymentStatusDto.PaymentStatus;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating payment status for ID: {OrderId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        try
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
            {
                return BadRequest("Cannot cancel shipped or delivered orders");
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling order with ID: {OrderId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    private string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }

    private decimal CalculateShippingCost(Entities.Order order)
    {
        // Simple shipping cost calculation - can be made more sophisticated
        return 10.00m; // Fixed shipping cost
    }

    private decimal CalculateTax(Entities.Order order)
    {
        // Simple tax calculation - can be made more sophisticated
        return order.TotalAmount * 0.09m; // 9% tax
    }
}

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
    public string? TrackingNumber { get; set; }
}

public class UpdatePaymentStatusDto
{
    public PaymentStatus PaymentStatus { get; set; }
}