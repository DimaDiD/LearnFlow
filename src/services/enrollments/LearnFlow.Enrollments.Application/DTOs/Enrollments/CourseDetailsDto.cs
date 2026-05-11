namespace LearnFlow.Enrollments.Application.DTOs.Enrollments;

public record CourseDetailsDto(
    string CourseId,
    string Title,
    string InstructorId,
    string Status,
    decimal Price
);