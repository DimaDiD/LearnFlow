using LearnFlow.Notifications.Application.Common.Interfaces;
using LearnFlow.Notifications.Domain.Entities;
using LearnFlow.Notifications.Domain.Enums;
using LearnFlow.Shared.Contracts.Events.Certificates;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Notifications.Application.Features.Notifications.Consumers;

public class CertificateIssuedConsumer : IConsumer<CertificateIssuedEvent>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<CertificateIssuedConsumer> _logger;

    public CertificateIssuedConsumer(
        INotificationRepository notificationRepository,
        IEmailService emailService,
        ILogger<CertificateIssuedConsumer> logger)
    {
        _notificationRepository = notificationRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CertificateIssuedEvent> context)
    {
        var message = context.Message;

        // Idempotency check
        var existing = await _notificationRepository.GetByStudentCourseAndTypeAsync(
            message.StudentId,
            message.CourseId,
            NotificationType.CertificateIssued,
            context.CancellationToken);

        if (existing is not null)
        {
            _logger.LogWarning(
                "Certificate notification already sent for student {StudentId} — skipping",
                message.StudentId);
            return;
        }

        var recipientEmail = $"{message.StudentId}@learnflow.com"; // placeholder
        var subject = $"Certificate Issued — {message.CourseTitleSnapshot}";

        try
        {
            await _emailService.SendCertificateIssuedAsync(
                recipientEmail,
                message.StudentId,
                message.CourseTitleSnapshot,
                message.CertificateNumber,
                context.CancellationToken);

            var notification = Notification.Create(
                message.StudentId,
                message.CourseId,
                recipientEmail,
                subject,
                NotificationType.CertificateIssued,
                message.CertificateNumber);

            await _notificationRepository.InsertAsync(notification, context.CancellationToken);

            _logger.LogInformation(
                "Certificate notification sent for student {StudentId} certificate {CertificateNumber}",
                message.StudentId, message.CertificateNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send certificate notification for student {StudentId}",
                message.StudentId);
            throw;
        }
    }
}