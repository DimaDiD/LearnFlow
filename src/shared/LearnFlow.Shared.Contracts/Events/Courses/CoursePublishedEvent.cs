namespace LearnFlow.Shared.Contracts.Events.Courses;

public record CoursePublishedEvent
{
    public string CourseId { get; init; } = string.Empty;
    public string InstructorId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = new();
    public DateTime PublishedAt { get; init; }

    public List<CourseModuleContract> Modules { get; init; } = new();

}

public record CourseModuleContract
{
    public string ModuleId { get; init; } = string.Empty;
    public List<string> LessonIds { get; init; } = new();
}