namespace LearnFlow.Identity.Application.DTOs.Auth;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    string UserId,
    string Email,
    string Role
);