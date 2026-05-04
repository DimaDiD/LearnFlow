using LearnFlow.Courses.Domain.Entities;

namespace LearnFlow.Courses.Application.Common.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<Course>> GetPublishedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<List<Course>> GetByInstructorIdAsync(string instructorId, CancellationToken ct = default);
    Task InsertAsync(Course course, CancellationToken ct = default);
    Task UpdateAsync(Course course, CancellationToken ct = default);
    Task<long> CountPublishedAsync(CancellationToken ct = default);
}