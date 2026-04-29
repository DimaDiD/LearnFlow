namespace LearnFlow.Shared.Contracts.Events.Courses;

public record CourseArchivedEvent
{
    public string CourseId { get; init; } = string.Empty;
    public DateTime ArchivedAt { get; init; }
}