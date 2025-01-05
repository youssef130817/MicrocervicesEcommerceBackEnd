using System.Text.Json;
using Cart.API.Models;
using StackExchange.Redis;

namespace Cart.API.Services;

public class RedisCartService : ICartService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private const string KeyPrefix = "cart:";

    public RedisCartService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task<ShoppingCart?> GetCartAsync(string cartId)
    {
        var key = $"{KeyPrefix}{cartId}";
        var data = await _database.StringGetAsync(key);

        if (data.IsNull)
            return null;

        return JsonSerializer.Deserialize<ShoppingCart>(data!);
    }

    public async Task UpdateCartAsync(ShoppingCart cart)
    {
        var key = $"{KeyPrefix}{cart.Id}";
        var data = JsonSerializer.Serialize(cart);

        // expiration de 30 jours
        await _database.StringSetAsync(key, data, TimeSpan.FromDays(30));
    }

    public async Task DeleteCartAsync(string cartId)
    {
        var key = $"{KeyPrefix}{cartId}";
        await _database.KeyDeleteAsync(key);
    }
}
