namespace LearnFlow.Courses.API.Controllers.Requests;

public record AddModuleRequest(string InstructorId, string Title, string Description, int Order);