using LearnFlow.Courses.Domain.Enums;

namespace LearnFlow.Courses.API.Controllers.Requests;

public record UpdateCourseRequest(string InstructorId, string Title, string Description, string Category, CourseLevel Level, List<string> Tags, decimal Price);
