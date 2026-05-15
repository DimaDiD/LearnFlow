using LearnFlow.Progress.Application.Common.Interfaces;
using LearnFlow.Progress.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Progress.Infrastructure.Persistence.Repositories;

public class CourseStructureRepository : ICourseStructureRepository
{
    private readonly IMongoCollection<CourseStructure> _courseStructures;

    public CourseStructureRepository(IMongoDatabase database)
    {
        _courseStructures = database.GetCollection<CourseStructure>("course_structures");
    }

    public Task<CourseStructure?> GetByCourseIdAsync(
        string courseId,
        CancellationToken ct = default)
        => _courseStructures
            .Find(s => s.CourseId == courseId)
            .FirstOrDefaultAsync(ct);

    public async Task UpsertAsync(CourseStructure structure, CancellationToken ct = default)
    {
        var filter = Builders<CourseStructure>.Filter.Eq(s => s.CourseId, structure.CourseId);
        var options = new ReplaceOptions { IsUpsert = true };
        await _courseStructures.ReplaceOneAsync(filter, structure, options, ct);
    }
}