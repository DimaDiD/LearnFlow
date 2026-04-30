using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Identity.Domain.Entities;

public class RefreshToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; private set; }

    [BsonElement("hash")]
    public string Hash { get; private set; }

    [BsonElement("expiresAt")]
    public DateTime ExpiresAt { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(string userId, string hash, int expiryDays = 7)
    {
        return new RefreshToken
        {
            Id = ObjectId.GenerateNewId().ToString(),
            UserId = userId,
            Hash = hash,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
}