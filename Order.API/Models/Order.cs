namespace Order.API.Models;

public class Order
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public ShippingAddress ShippingAddress { get; set; } = new();
    public string? TrackingNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum OrderStatus
{
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}
