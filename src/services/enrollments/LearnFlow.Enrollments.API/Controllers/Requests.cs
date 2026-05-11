namespace LearnFlow.Enrollments.API.Controllers;

public record EnrollStudentRequest(string StudentId, string CourseId);
public record CancelEnrollmentRequest(string StudentId);