using LearnFlow.Enrollments.Application.DTOs.Enrollments;
using MediatR;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Queries.GetCourseEnrollments;

public record GetCourseEnrollmentsQuery(
    string CourseId,
    string InstructorId
) : IRequest<List<EnrollmentDto>>;