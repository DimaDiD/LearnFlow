namespace LearnFlow.Shared.Contracts.Events.Identity;

public record UserRegisteredEvent
{
    public string UserId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime RegisteredAt { get; init; }
}