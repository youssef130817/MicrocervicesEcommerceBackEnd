using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Data;
using Order.API.Models;

namespace Order.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    private static OrderResponse MapToResponse(Models.Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                ImageUrl = item.ImageUrl
            }).ToList(),
            ShippingAddress = new ShippingAddressResponse
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                ZipCode = order.ShippingAddress.ZipCode,
                PhoneNumber = order.ShippingAddress.PhoneNumber
            }
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .AsQueryable();

        // Apply status filter
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
        {
            query = query.Where(o => o.Status == orderStatus);
        }

        // Apply pagination
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            orders = orders.Select(MapToResponse),
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalPages,
                totalItems
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(order));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request.CartItems == null || !request.CartItems.Any())
        {
            return BadRequest(new { message = "Cart is empty" });
        }

        // Calculate total amount
        decimal totalAmount = request.CartItems.Sum(item => item.UnitPrice * item.Quantity);

        // Create order
        var order = new Models.Order
        {
            Status = OrderStatus.Confirmed,
            TotalAmount = totalAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ShippingAddress = new ShippingAddress
            {
                Street = request.ShippingAddress.Street,
                City = request.ShippingAddress.City,
                State = request.ShippingAddress.State,
                ZipCode = request.ShippingAddress.ZipCode,
                PhoneNumber = request.ShippingAddress.PhoneNumber
            }
        };

        // Add order items
        foreach (var item in request.CartItems)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                ImageUrl = item.ImageUrl
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapToResponse(order));
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Confirmed)
        {
            return BadRequest(new { message = "Order cannot be cancelled" });
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order cancelled successfully" });
    }

    [HttpGet("{id}/tracking")]
    public async Task<IActionResult> GetOrderTracking(Guid id)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            order.Id,
            order.Status,
            order.TrackingNumber,
            order.UpdatedAt
        });
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAllOrders()
    {
        try
        {
            // Supprimer d'abord les éléments liés
            await _context.OrderItems.ExecuteDeleteAsync();
            await _context.ShippingAddresses.ExecuteDeleteAsync();
            await _context.Orders.ExecuteDeleteAsync();

            return Ok(new { message = "Toutes les commandes ont été supprimées avec succès" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Une erreur est survenue lors de la suppression des commandes", error = ex.Message });
        }
    }
}

public class CartItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
}

public class CreateOrderRequest
{
    public List<CartItemRequest> CartItems { get; set; } = new();
    public ShippingAddressRequest ShippingAddress { get; set; } = new();
}

public class ShippingAddressRequest
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class OrderResponse
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
    public ShippingAddressResponse ShippingAddress { get; set; } = new();
}

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
}

public class ShippingAddressResponse
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
