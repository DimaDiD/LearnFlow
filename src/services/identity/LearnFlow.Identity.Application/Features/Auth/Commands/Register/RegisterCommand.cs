using LearnFlow.Identity.Application.DTOs.Auth;
using LearnFlow.Identity.Domain.Enums;
using MediatR;

namespace LearnFlow.Identity.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role
) : IRequest<AuthResponseDto>;