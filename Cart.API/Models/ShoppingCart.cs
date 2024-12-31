namespace Cart.API.Models;

public class ShoppingCart
{
    public string Id { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CartItem
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}