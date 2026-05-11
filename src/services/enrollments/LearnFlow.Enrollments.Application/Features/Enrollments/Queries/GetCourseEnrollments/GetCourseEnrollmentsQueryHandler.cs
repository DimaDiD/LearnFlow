using LearnFlow.Enrollments.Application.Common.Interfaces;
using LearnFlow.Enrollments.Application.DTOs.Enrollments;
using MediatR;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Queries.GetCourseEnrollments;

public class GetCourseEnrollmentsQueryHandler
    : IRequestHandler<GetCourseEnrollmentsQuery, List<EnrollmentDto>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetCourseEnrollmentsQueryHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<EnrollmentDto>> Handle(
        GetCourseEnrollmentsQuery query,
        CancellationToken ct)
    {
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(query.CourseId, ct);

        if (enrollments.Any() && enrollments.First().InstructorId != query.InstructorId)
            throw new UnauthorizedAccessException("You can only view enrollments for your own courses.");

        return enrollments.Select(e => new EnrollmentDto(
            e.Id,
            e.StudentId,
            e.CourseId,
            e.CourseTitleSnapshot,
            e.Status.ToString(),
            e.EnrolledAt)).ToList();
    }
}