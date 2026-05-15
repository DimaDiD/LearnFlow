namespace LearnFlow.Shared.Contracts.Events.Courses;

public record CourseUpdatedEvent
{
    public string CourseId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = new();
    public DateTime UpdatedAt { get; init; }
    public List<CourseModuleContract> Modules { get; init; } = new();
}