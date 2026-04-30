using LearnFlow.Identity.Application.DTOs.Auth;
using MediatR;

namespace LearnFlow.Identity.Application.Features.Auth.Commands.Refresh;

public record RefreshTokenCommand(
    string UserId,
    string RefreshToken
) : IRequest<AuthResponseDto>;