namespace LearnFlow.Courses.API.Controllers.Requests;

public record AddLessonRequest(string InstructorId, string Title, string Description, string VideoUrl, int DurationMinutes, int Order);
