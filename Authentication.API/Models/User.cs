using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.API.Models;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("Email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("LastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("Role")]
    public string Role { get; set; } = "User";

    [BsonElement("ResetPasswordToken")]
    public string? ResetPasswordToken { get; set; }

    [BsonElement("ResetPasswordTokenExpiry")]
    public DateTime? ResetPasswordTokenExpiry { get; set; }

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("UpdatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}