namespace LearnFlow.Shared.Contracts.Events.Progress;

public record CourseCompletedEvent
{
    public string ProgressId { get; init; } = string.Empty;
    public string StudentId { get; init; } = string.Empty;
    public string CourseId { get; init; } = string.Empty;
    public string CourseTitleSnapshot { get; init; } = string.Empty;
    public DateTime CompletedAt { get; init; }
}