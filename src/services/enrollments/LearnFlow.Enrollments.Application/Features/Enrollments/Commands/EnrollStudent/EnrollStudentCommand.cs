using MediatR;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Commands.EnrollStudent;

public record EnrollStudentCommand(
    string StudentId,
    string CourseId
) : IRequest<string>;