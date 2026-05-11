using LearnFlow.Enrollments.Application.DTOs.Enrollments;
using MediatR;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Queries.GetMyEnrollments;

public record GetMyEnrollmentsQuery(string StudentId) : IRequest<List<EnrollmentDto>>;