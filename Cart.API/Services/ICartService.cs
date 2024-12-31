using Cart.API.Models;

namespace Cart.API.Services;

public interface ICartService
{
    Task<ShoppingCart?> GetCartAsync(string cartId);
    Task UpdateCartAsync(ShoppingCart cart);
    Task DeleteCartAsync(string cartId);
}