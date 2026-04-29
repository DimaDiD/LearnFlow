namespace LearnFlow.Shared.Contracts.Events.Enrollments;

public record StudentEnrolledEvent
{
    public string EnrollmentId { get; init; } = string.Empty;
    public string StudentId { get; init; } = string.Empty;
    public string CourseId { get; init; } = string.Empty;
    public string CourseTitleSnapshot { get; init; } = string.Empty;
    public DateTime EnrolledAt { get; init; }
}