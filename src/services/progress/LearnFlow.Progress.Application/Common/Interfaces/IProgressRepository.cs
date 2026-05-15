using ProgressEntity = LearnFlow.Progress.Domain.Entities.Progress;

namespace LearnFlow.Progress.Application.Common.Interfaces;

public interface IProgressRepository
{
    Task<ProgressEntity?> GetByStudentAndCourseAsync(string studentId, string courseId, CancellationToken ct = default);
    Task<List<ProgressEntity>> GetByStudentIdAsync(string studentId, CancellationToken ct = default);
    Task InsertAsync(ProgressEntity progress, CancellationToken ct = default);
    Task UpdateAsync(ProgressEntity progress, CancellationToken ct = default);
}