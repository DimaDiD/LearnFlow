using LearnFlow.Enrollments.Application.DTOs.Enrollments;

namespace LearnFlow.Enrollments.Application.Common.Interfaces;

public interface ICourseServiceClient
{
    Task<CourseDetailsDto?> GetCourseAsync(string courseId, CancellationToken ct = default);
}