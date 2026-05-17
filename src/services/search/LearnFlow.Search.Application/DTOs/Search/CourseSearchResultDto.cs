namespace LearnFlow.Search.Application.DTOs.Search;

public record CourseSearchResultDto(
    string Id,
    string Title,
    string Description,
    string Category,
    string Level,
    List<string> Tags,
    string InstructorId,
    string InstructorName,
    decimal Price,
    int EnrollmentCount,
    DateTime PublishedAt
);