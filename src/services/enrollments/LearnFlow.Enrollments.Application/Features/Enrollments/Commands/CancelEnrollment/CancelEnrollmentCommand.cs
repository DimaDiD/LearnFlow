using MediatR;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Commands.CancelEnrollment;

public record CancelEnrollmentCommand(
    string EnrollmentId,
    string StudentId
) : IRequest;