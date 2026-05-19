using LearnFlow.Notifications.Domain.Entities;

namespace LearnFlow.Certificates.Application.Common.Interfaces;

public interface ICertificateRepository
{

    Task<Certificate?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Certificate?> GetByStudentAndCourseAsync(string studentId, string courseId, CancellationToken ct = default);
    Task<List<Certificate>> GetByStudentIdAsync(string studentId, int page, int pageSize, CancellationToken ct = default);
    Task InsertAsync(Certificate enrollment, CancellationToken ct = default);
}
