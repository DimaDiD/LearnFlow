namespace LearnFlow.Courses.Application.DTOs.Courses;

public record ModuleDto(
    string Id,
    string Title,
    string Description,
    int Order,
    List<LessonDto> Lessons
);