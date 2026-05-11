using LearnFlow.Enrollments.Application.Common.Interfaces;
using LearnFlow.Enrollments.Application.DTOs.Enrollments;
using MediatR;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Queries.GetMyEnrollments;

public class GetMyEnrollmentsQueryHandler
    : IRequestHandler<GetMyEnrollmentsQuery, List<EnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetMyEnrollmentsQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<EnrollmentDto>> Handle(
        GetMyEnrollmentsQuery query,
        CancellationToken ct)
    {
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(query.StudentId, ct);

        return enrollments.Select(e => new EnrollmentDto(
            e.Id,
            e.StudentId,
            e.CourseId,
            e.CourseTitleSnapshot,
            e.Status.ToString(),
            e.EnrolledAt)).ToList();
    }
}