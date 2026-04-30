using LearnFlow.Identity.Application.Common.Interfaces;
using LearnFlow.Identity.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Identity.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _tokens;

    public RefreshTokenRepository(IMongoDatabase database)
    {
        _tokens = database.GetCollection<RefreshToken>("refresh_tokens");
    }

    public async Task<RefreshToken?> GetByUserIdAndVerifyAsync(
        string userId,
        string rawToken,
        CancellationToken ct = default)
    {
        var tokens = await _tokens
            .Find(t => t.UserId == userId && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        return tokens.FirstOrDefault(t => BCrypt.Net.BCrypt.Verify(rawToken, t.Hash));
    }

    public Task InsertAsync(RefreshToken token, CancellationToken ct = default)
        => _tokens.InsertOneAsync(token, cancellationToken: ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _tokens.DeleteOneAsync(t => t.Id == id, ct);

    public Task DeleteAllForUserAsync(string userId, CancellationToken ct = default)
        => _tokens.DeleteManyAsync(t => t.UserId == userId, ct);
}