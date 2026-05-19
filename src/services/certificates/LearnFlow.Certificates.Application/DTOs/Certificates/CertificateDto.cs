namespace LearnFlow.Certificates.Application.DTOs.Certificates;
public record CertificateDto(
    string Id,
    string CertificateNumber,
    string StudentId,
    string CourseId,
    string CourseTitleSnapshot,
    decimal CompletionPercentage,
    DateTime IssuedAt
);