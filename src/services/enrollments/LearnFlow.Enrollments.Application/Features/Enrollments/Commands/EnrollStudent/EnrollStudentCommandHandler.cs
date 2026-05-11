using LearnFlow.Enrollments.Application.Common.Interfaces;
using LearnFlow.Enrollments.Domain.Entities;
using LearnFlow.Shared.Contracts.Events.Enrollments;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Enrollments.Application.Features.Enrollments.Commands.EnrollStudent;

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, string>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseServiceClient _courseServiceClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EnrollStudentCommandHandler> _logger;

    public EnrollStudentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseServiceClient courseServiceClient,
        IPublishEndpoint publishEndpoint,
        ILogger<EnrollStudentCommandHandler> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseServiceClient = courseServiceClient;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<string> Handle(EnrollStudentCommand command, CancellationToken ct)
    {
        var course = await _courseServiceClient.GetCourseAsync(command.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {command.CourseId} not found.");

        if (course.Status != "Published")
            throw new InvalidOperationException("Cannot enroll in a course that is not published.");

        if (course.InstructorId == command.StudentId)
            throw new InvalidOperationException("Instructors cannot enroll in their own courses.");

        if (course.Price > 0)
            throw new InvalidOperationException("Paid courses require payment processing.");

        var existing = await _enrollmentRepository
            .GetByStudentAndCourseAsync(command.StudentId, command.CourseId, ct);

        if (existing is not null)
        {
            if (existing.Status == Domain.Enums.EnrollmentStatus.Active)
                throw new InvalidOperationException("Student is already enrolled in this course.");

            existing.Reactivate();
            await _enrollmentRepository.UpdateAsync(existing, ct);

            _logger.LogInformation(
                "Enrollment reactivated for student {StudentId} course {CourseId}",
                command.StudentId, command.CourseId);

            return existing.Id;
        }

        var enrollment = Enrollment.Create(
            command.StudentId,
            command.CourseId,
            course.InstructorId,
            course.Title);

        await _enrollmentRepository.InsertAsync(enrollment, ct);

        await _publishEndpoint.Publish(new StudentEnrolledEvent
        {
            EnrollmentId = enrollment.Id,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            CourseTitleSnapshot = enrollment.CourseTitleSnapshot,
            EnrolledAt = enrollment.EnrolledAt
        }, ct);

        _logger.LogInformation(
            "Student {StudentId} enrolled in course {CourseId}",
            command.StudentId, command.CourseId);

        return enrollment.Id;
    }
}