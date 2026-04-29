namespace LearnFlow.Shared.Contracts.Events.Certificates;

public record CertificateIssuedEvent
{
    public string CertificateId { get; init; } = string.Empty;
    public string CertificateNumber { get; init; } = string.Empty;
    public string StudentId { get; init; } = string.Empty;
    public string CourseId { get; init; } = string.Empty;
    public string CourseTitleSnapshot { get; init; } = string.Empty;
    public DateTime IssuedAt { get; init; }
}