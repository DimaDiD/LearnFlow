namespace LearnFlow.Courses.Application.DTOs.Courses;

public record LessonDto(
    string Id,
    string Title,
    string Description,
    string VideoUrl,
    int DurationMinutes,
    int Order
);