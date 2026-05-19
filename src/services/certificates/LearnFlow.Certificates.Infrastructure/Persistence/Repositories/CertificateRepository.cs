using LearnFlow.Certificates.Application.Common.Interfaces;
using LearnFlow.Notifications.Domain.Entities;
using MongoDB.Driver;

namespace LearnFlow.Certificates.Infrastructure.Persistence.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private readonly IMongoCollection<Certificate> _certificates;

    public CertificateRepository(IMongoDatabase database)
    {
        _certificates = database.GetCollection<Certificate>("certificates");
    }

    public Task<Certificate?> GetByIdAsync(string id, CancellationToken ct = default)
        => _certificates.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public Task<Certificate?> GetByStudentAndCourseAsync(
        string studentId,
        string courseId,
        CancellationToken ct = default)
        => _certificates
            .Find(c => c.StudentId == studentId && c.CourseId == courseId)
            .FirstOrDefaultAsync(ct);

    public Task<List<Certificate>> GetByStudentIdAsync(
        string studentId,
        int page,
        int pageSize,
        CancellationToken ct = default)
        => _certificates
            .Find(c => c.StudentId == studentId)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .SortByDescending(c => c.IssuedAt)
            .ToListAsync(ct);

    public Task InsertAsync(Certificate certificate, CancellationToken ct = default)
        => _certificates.InsertOneAsync(certificate, cancellationToken: ct);
}