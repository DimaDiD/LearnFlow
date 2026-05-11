using LearnFlow.Enrollments.Domain.Entities;

namespace LearnFlow.Enrollments.Application.Common.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, string courseId, CancellationToken ct = default);
    Task<List<Enrollment>> GetByStudentIdAsync(string studentId, CancellationToken ct = default);
    Task<List<Enrollment>> GetByCourseIdAsync(string courseId, CancellationToken ct = default);
    Task InsertAsync(Enrollment enrollment, CancellationToken ct = default);
    Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default);
}