using LearnFlow.Identity.Domain.Entities;

namespace LearnFlow.Identity.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByUserIdAndVerifyAsync(string userId, string rawToken, CancellationToken ct = default);
    Task InsertAsync(RefreshToken token, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task DeleteAllForUserAsync(string userId, CancellationToken ct = default);
}