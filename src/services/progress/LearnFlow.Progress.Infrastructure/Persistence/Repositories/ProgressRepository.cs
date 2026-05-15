using LearnFlow.Progress.Application.Common.Interfaces;
using MongoDB.Driver;
using ProgressEntity = LearnFlow.Progress.Domain.Entities.Progress;

namespace LearnFlow.Progress.Infrastructure.Persistence.Repositories;

public class ProgressRepository : IProgressRepository
{
    private readonly IMongoCollection<ProgressEntity> _progress;

    public ProgressRepository(IMongoDatabase database)
    {
        _progress = database.GetCollection<ProgressEntity>("progress");
    }

    public Task<ProgressEntity?> GetByStudentAndCourseAsync(
        string studentId,
        string courseId,
        CancellationToken ct = default)
        => _progress
            .Find(p => p.StudentId == studentId && p.CourseId == courseId)
            .FirstOrDefaultAsync(ct);

    public Task<List<ProgressEntity>> GetByStudentIdAsync(
        string studentId,
        CancellationToken ct = default)
        => _progress
            .Find(p => p.StudentId == studentId)
            .SortByDescending(p => p.StartedAt)
            .ToListAsync(ct);

    public Task InsertAsync(ProgressEntity progress, CancellationToken ct = default)
        => _progress.InsertOneAsync(progress, cancellationToken: ct);

    public Task UpdateAsync(ProgressEntity progress, CancellationToken ct = default)
        => _progress.ReplaceOneAsync(
            p => p.Id == progress.Id,
            progress,
            cancellationToken: ct);
}