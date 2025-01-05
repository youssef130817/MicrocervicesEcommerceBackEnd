using Microsoft.EntityFrameworkCore;
using Order.API.Data;
using Order.API.Models;

namespace Order.API.Services;

public class OrderService
{
    private readonly OrderDbContext _context;

    public OrderService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<List<Models.Order>> GetOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .ToListAsync();
    }

    public async Task<Models.Order?> GetOrderAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Models.Order> CreateOrderAsync(Models.Order order)
    {
        order.TrackingNumber = TrackingNumberGenerator.GenerateTrackingNumber();
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Models.Order?> UpdateOrderAsync(Guid id, Models.Order order)
    {
        var existingOrder = await GetOrderAsync(id);
        if (existingOrder == null) return null;

        existingOrder.Status = order.Status;
        existingOrder.TrackingNumber = order.TrackingNumber;
        existingOrder.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingOrder;
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}