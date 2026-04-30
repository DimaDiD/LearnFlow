using LearnFlow.Identity.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Identity.Infrastructure.Persistence;

public static class MongoDbInitializer
{
    public static async Task InitializeAsync(IMongoDatabase database)
    {
        await CreateUserIndexesAsync(database);
        await CreateRefreshTokenIndexesAsync(database);
    }

    private static async Task CreateUserIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<User>("users");

        var emailIndex = Builders<User>.IndexKeys.Ascending(u => u.Email);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                emailIndex,
                new CreateIndexOptions { Unique = true, Name = "idx_users_email_unique" }));
    }

    private static async Task CreateRefreshTokenIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<RefreshToken>("refresh_tokens");

        var userIdIndex = Builders<RefreshToken>.IndexKeys.Ascending(t => t.UserId);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<RefreshToken>(
                userIdIndex,
                new CreateIndexOptions { Name = "idx_refresh_tokens_userId" }));

        // TTL index — MongoDB automatically deletes expired tokens
        var ttlIndex = Builders<RefreshToken>.IndexKeys.Ascending(t => t.ExpiresAt);
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<RefreshToken>(
                ttlIndex,
                new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.Zero,
                    Name = "idx_refresh_tokens_ttl"
                }));
    }
}