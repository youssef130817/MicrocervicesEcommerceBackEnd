namespace Order.API.Models.DTOs;

public class CreateOrderDTO
{
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CreateOrderItemDTO> Items { get; set; } = new();
    public CreateShippingAddressDTO ShippingAddress { get; set; } = new();
}

public class CreateOrderItemDTO
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateShippingAddressDTO
{
    public string FullName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}