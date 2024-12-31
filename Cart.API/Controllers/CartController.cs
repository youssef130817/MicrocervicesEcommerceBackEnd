using Cart.API.Models;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string GetCartId()
    {
        var cartId = Request.Headers["X-Cart-ID"].ToString();
        if (string.IsNullOrEmpty(cartId))
        {
            throw new InvalidOperationException("Cart ID is required");
        }
        return cartId;
    }

    [HttpGet("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> GetCart(string cartId)
    {
        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null)
        {
            cart = new ShoppingCart
            {
                Id = cartId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Items = new List<CartItem>()
            };
            await _cartService.UpdateCartAsync(cart);
        }
        return cart;
    }

    [HttpPost("{cartId}/items")]
    public async Task<ActionResult<ShoppingCart>> AddItem(string cartId, [FromBody] CartItem item)
    {
        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null)
        {
            cart = new ShoppingCart
            {
                Id = cartId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Items = new List<CartItem>()
            };
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            item.Id = Guid.NewGuid().ToString();
            cart.Items.Add(item);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartService.UpdateCartAsync(cart);
        return cart;
    }

    [HttpPut("{cartId}/items/{productId}")]
    public async Task<ActionResult<ShoppingCart>> UpdateItemQuantity(string cartId, string productId, [FromBody] CartItem item)
    {
        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null)
            return NotFound();

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem == null)
            return NotFound();

        existingItem.Quantity = item.Quantity;
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartService.UpdateCartAsync(cart);
        return cart;
    }

    [HttpDelete("{cartId}/items/{productId}")]
    public async Task<ActionResult> RemoveItem(string cartId, string productId)
    {
        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null)
            return NotFound();

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartService.UpdateCartAsync(cart);
        }

        return Ok();
    }

    [HttpPost("{cartId}/clear")]
    public async Task<ActionResult> ClearCart(string cartId)
    {
        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null)
            return NotFound();

        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartService.UpdateCartAsync(cart);
        return Ok();
    }
}
