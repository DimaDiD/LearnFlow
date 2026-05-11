using LearnFlow.Enrollments.Application.Common.Interfaces;
using LearnFlow.Enrollments.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Enrollments.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly IMongoCollection<Enrollment> _enrollments;

    public EnrollmentRepository(IMongoDatabase database)
    {
        _enrollments = database.GetCollection<Enrollment>("enrollments");
    }

    public Task<Enrollment?> GetByIdAsync(string id, CancellationToken ct = default)
        => _enrollments.Find(e => e.Id == id).FirstOrDefaultAsync(ct);

    public Task<Enrollment?> GetByStudentAndCourseAsync(
        string studentId,
        string courseId,
        CancellationToken ct = default)
        => _enrollments
            .Find(e => e.StudentId == studentId && e.CourseId == courseId)
            .FirstOrDefaultAsync(ct);

    public Task<List<Enrollment>> GetByStudentIdAsync(
        string studentId,
        CancellationToken ct = default)
        => _enrollments
            .Find(e => e.StudentId == studentId)
            .SortByDescending(e => e.EnrolledAt)
            .ToListAsync(ct);

    public Task<List<Enrollment>> GetByCourseIdAsync(
        string courseId,
        CancellationToken ct = default)
        => _enrollments
            .Find(e => e.CourseId == courseId)
            .SortByDescending(e => e.EnrolledAt)
            .ToListAsync(ct);

    public Task InsertAsync(Enrollment enrollment, CancellationToken ct = default)
        => _enrollments.InsertOneAsync(enrollment, cancellationToken: ct);

    public Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default)
        => _enrollments.ReplaceOneAsync(
            e => e.Id == enrollment.Id,
            enrollment,
            cancellationToken: ct);
}