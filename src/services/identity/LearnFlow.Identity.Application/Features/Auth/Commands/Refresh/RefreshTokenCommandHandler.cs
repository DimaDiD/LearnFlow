using LearnFlow.Identity.Application.Common.Interfaces;
using LearnFlow.Identity.Application.DTOs.Auth;
using LearnFlow.Identity.Domain.Entities;
using MediatR;

namespace LearnFlow.Identity.Application.Features.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, ct);
        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid token.");
     
        var storedToken = await _refreshTokenRepository
            .GetByUserIdAndVerifyAsync(command.UserId, command.RefreshToken, ct);

        if (storedToken is null || storedToken.IsExpired())
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        await _refreshTokenRepository.DeleteAsync(storedToken.Id, ct);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var rawRefreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(rawRefreshToken);

        var newRefreshToken = RefreshToken.Create(user.Id, refreshTokenHash);
        await _refreshTokenRepository.InsertAsync(newRefreshToken, ct);

        return new AuthResponseDto(
            accessToken,
            rawRefreshToken,
            user.Id,
            user.Email,
            user.Role.ToString());
    }
}