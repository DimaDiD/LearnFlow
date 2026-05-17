using LearnFlow.Search.Domain.Entities;

namespace LearnFlow.Search.Application.Common.Interfaces;

public interface ISearchRepository
{
    Task IndexCourseAsync(CourseDocument course, CancellationToken ct = default);
    Task UpdateCourseAsync(CourseDocument course, CancellationToken ct = default);
    Task DeleteCourseAsync(string courseId, CancellationToken ct = default);
    Task<(List<CourseDocument> Items, long Total)> SearchAsync(
        string? query,
        string? category,
        string? level,
        int page,
        int pageSize,
        string? sortBy,
        CancellationToken ct = default);
    Task UpsertInstructorProfileAsync(string userId, string fullName, CancellationToken ct = default);
    Task<string?> GetInstructorNameAsync(string userId, CancellationToken ct = default);
}