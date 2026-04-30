using LearnFlow.Identity.Domain.Entities;

namespace LearnFlow.Identity.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}