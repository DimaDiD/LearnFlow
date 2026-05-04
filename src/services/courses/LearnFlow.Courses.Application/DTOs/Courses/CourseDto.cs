using LearnFlow.Courses.Domain.Enums;

namespace LearnFlow.Courses.Application.DTOs.Courses;

public record CourseDto(
    string Id,
    string Title,
    string Description,
    string InstructorId,
    string Category,
    CourseLevel Level,
    List<string> Tags,
    decimal Price,
    CourseStatus Status,
    List<ModuleDto> Modules,
    int TotalLessons,
    int TotalDurationMinutes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? PublishedAt
);