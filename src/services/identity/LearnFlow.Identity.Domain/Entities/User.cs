using LearnFlow.Identity.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LearnFlow.Identity.Domain.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    [BsonElement("email")]
    public string Email { get; private set; }

    [BsonElement("passwordHash")]
    public string PasswordHash { get; private set; }

    [BsonElement("firstName")]
    public string FirstName { get; private set; }

    [BsonElement("lastName")]
    public string LastName { get; private set; }

    [BsonElement("role")]
    [BsonRepresentation(BsonType.String)]
    public UserRole Role { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("isActive")]
    public bool IsActive { get; private set; }

    [BsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    private User() { }

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        UserRole role)
    {
        return new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
}