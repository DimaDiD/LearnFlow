using LearnFlow.Identity.Application.DTOs.Auth;
using MediatR;

namespace LearnFlow.Identity.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;