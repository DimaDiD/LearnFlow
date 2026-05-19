using LearnFlow.Notifications.Application.Common.Interfaces;
using LearnFlow.Notifications.Domain.Entities;
using LearnFlow.Notifications.Domain.Enums;
using LearnFlow.Shared.Contracts.Events.Enrollments;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Notifications.Application.Features.Notifications.Consumers;

public class StudentEnrolledConsumer : IConsumer<StudentEnrolledEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<StudentEnrolledConsumer> _logger;

    public StudentEnrolledConsumer(
        INotificationRepository notificationRepository,
        IEmailService emailService,
        ILogger<StudentEnrolledConsumer> logger)
    {
        _notificationRepository = notificationRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StudentEnrolledEvent> context)
    {
        var message = context.Message;


        var existing = await _notificationRepository.GetByStudentCourseAndTypeAsync(
            message.StudentId,
            message.CourseId,
            NotificationType.EnrollmentConfirmation,
            context.CancellationToken);

        if (existing is not null)
        {
            _logger.LogWarning(
                "Enrollment confirmation already sent for student {StudentId} course {CourseId} — skipping",
                message.StudentId, message.CourseId);
            return;
        }

        var recipientEmail = $"{message.StudentId}@learnflow.com";
        var subject = $"Enrollment Confirmation — {message.CourseTitleSnapshot}";

        try
        {
            await _emailService.SendEnrollmentConfirmationAsync(
                recipientEmail,
                message.StudentId,
                message.CourseTitleSnapshot,
                context.CancellationToken);

            var notification = Notification.Create(
                message.StudentId,
                message.CourseId,
                recipientEmail,
                subject,
                NotificationType.EnrollmentConfirmation);

            await _notificationRepository.InsertAsync(notification, context.CancellationToken);

            _logger.LogInformation(
                "Enrollment confirmation sent for student {StudentId} course {CourseId}",
                message.StudentId, message.CourseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send enrollment confirmation for student {StudentId}",
                message.StudentId);
            throw;
        }
    }
}