using LearnFlow.Progress.Domain.Entities;

namespace LearnFlow.Progress.Application.Common.Interfaces;

public interface ICourseStructureRepository
{
    Task<CourseStructure?> GetByCourseIdAsync(string courseId, CancellationToken ct = default);
    Task UpsertAsync(CourseStructure structure, CancellationToken ct = default);
}