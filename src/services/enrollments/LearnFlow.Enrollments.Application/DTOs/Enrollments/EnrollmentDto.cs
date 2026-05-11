namespace LearnFlow.Enrollments.Application.DTOs.Enrollments;

public record EnrollmentDto(
    string Id,
    string StudentId,
    string CourseId,
    string CourseTitleSnapshot,
    string Status,
    DateTime EnrolledAt
);