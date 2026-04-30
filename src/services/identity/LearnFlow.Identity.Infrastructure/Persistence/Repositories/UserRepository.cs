using LearnFlow.Identity.Application.Common.Interfaces;
using LearnFlow.Identity.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Identity.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
        => _users.Find(u => u.Id == id).FirstOrDefaultAsync(ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _users.Find(u => u.Email == email.ToLowerInvariant()).FirstOrDefaultAsync(ct);

    public Task InsertAsync(User user, CancellationToken ct = default)
        => _users.InsertOneAsync(user, cancellationToken: ct);

    public Task UpdateAsync(User user, CancellationToken ct = default)
        => _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);
}