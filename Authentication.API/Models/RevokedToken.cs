using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.API.Models;

public class RevokedToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime RevokedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }
}