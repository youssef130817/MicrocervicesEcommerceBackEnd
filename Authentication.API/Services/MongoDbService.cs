using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Authentication.API.Models;

namespace Authentication.API.Services;

public class MongoDbService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<RevokedToken> _revokedTokens;

    public MongoDbService(IConfiguration configuration)
    {
        var mongoClient = new MongoClient(configuration["MongoDB:ConnectionString"]);
        var database = mongoClient.GetDatabase(configuration["MongoDB:DatabaseName"]);
        _users = database.GetCollection<User>("Users");
        _revokedTokens = database.GetCollection<RevokedToken>("RevokedTokens");
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserById(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateUser(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateUser(string id, User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == id, user);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    public async Task AddRevokedToken(RevokedToken token)
    {
        await _revokedTokens.InsertOneAsync(token);

        // Nettoyer les tokens expirés
        var filter = Builders<RevokedToken>.Filter.Lt(t => t.ExpiresAt, DateTime.UtcNow);
        await _revokedTokens.DeleteManyAsync(filter);
    }

    public async Task<bool> IsTokenRevoked(string token)
    {
        var revokedToken = await _revokedTokens.Find(t => t.Token == token).FirstOrDefaultAsync();
        return revokedToken != null;
    }
}