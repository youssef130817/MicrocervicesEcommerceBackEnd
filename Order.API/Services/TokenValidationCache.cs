using System.Collections.Concurrent;
using Order.API.Models;

namespace Order.API.Services;

public class TokenValidationCache
{
    private readonly ConcurrentDictionary<string, TokenValidationMessage> _cache = new();

    public void UpdateTokenStatus(string requestId, TokenValidationMessage validationMessage)
    {
        _cache.AddOrUpdate(requestId, validationMessage, (_, _) => validationMessage);
    }

    public TokenValidationMessage? GetTokenStatus(string requestId)
    {
        _cache.TryGetValue(requestId, out var status);
        return status;
    }

    public void RemoveToken(string requestId)
    {
        _cache.TryRemove(requestId, out _);
    }
}