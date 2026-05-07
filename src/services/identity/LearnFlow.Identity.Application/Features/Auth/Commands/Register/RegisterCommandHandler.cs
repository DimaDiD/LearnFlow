using LearnFlow.Identity.Application.Common.Interfaces;
using LearnFlow.Identity.Application.DTOs.Auth;
using LearnFlow.Identity.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Identity.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existing = await _userRepository.GetByEmailAsync(command.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Email {command.Email} is already registered.");
     
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var user = User.Create(
            command.Email,
            passwordHash,
            command.FirstName,
            command.LastName,
            command.Role);

        await _userRepository.InsertAsync(user, ct);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var rawRefreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(rawRefreshToken);

        var refreshToken = RefreshToken.Create(user.Id, refreshTokenHash);
        await _refreshTokenRepository.InsertAsync(refreshToken, ct);

        _logger.LogInformation(
            "User registered: {UserId} {Email} {Role}",
            user.Id,
            user.Email, 
            user.Role);

        return new AuthResponseDto(
            accessToken,
            rawRefreshToken,
            user.Id,
            user.Email,
            user.Role.ToString());
    }
}