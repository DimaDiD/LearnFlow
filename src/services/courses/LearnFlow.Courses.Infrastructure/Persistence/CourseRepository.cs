using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Domain.Entities;
using LearnFlow.Courses.Domain.Enums;
using MongoDB.Driver;

namespace LearnFlow.Courses.Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly IMongoCollection<Course> _courses;

    public CourseRepository(IMongoDatabase database)
    {
        _courses = database.GetCollection<Course>("courses");
    }

    public Task<Course?> GetByIdAsync(string id, CancellationToken ct = default)
        => _courses.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public Task<List<Course>> GetPublishedAsync(
        int page,
        int pageSize,
        CancellationToken ct = default)
        => _courses
            .Find(c => c.Status == CourseStatus.Published)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .SortByDescending(c => c.PublishedAt)
            .ToListAsync(ct);

    public Task<List<Course>> GetByInstructorIdAsync(
        string instructorId,
        CancellationToken ct = default)
        => _courses
            .Find(c => c.InstructorId == instructorId)
            .SortByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    public Task InsertAsync(Course course, CancellationToken ct = default)
        => _courses.InsertOneAsync(course, cancellationToken: ct);

    public Task UpdateAsync(Course course, CancellationToken ct = default)
        => _courses.ReplaceOneAsync(c => c.Id == course.Id, course, cancellationToken: ct);

    public Task<long> CountPublishedAsync(CancellationToken ct = default)
        => _courses.CountDocumentsAsync(c => c.Status == CourseStatus.Published, cancellationToken: ct);
}