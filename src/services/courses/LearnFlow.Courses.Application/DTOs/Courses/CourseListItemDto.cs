using LearnFlow.Courses.Domain.Enums;

namespace LearnFlow.Courses.Application.DTOs.Courses;

public record CourseListItemDto(
    string Id,
    string Title,
    string Category,
    CourseLevel Level,
    decimal Price,
    string InstructorId,
    CourseStatus Status,
    int TotalLessons,
    int TotalDurationMinutes,
    DateTime? PublishedAt
);