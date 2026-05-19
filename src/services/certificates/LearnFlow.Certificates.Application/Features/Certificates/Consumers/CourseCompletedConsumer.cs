using LearnFlow.Certificates.Application.Common.Interfaces;
using LearnFlow.Notifications.Domain.Entities;
using LearnFlow.Shared.Contracts.Events.Certificates;
using LearnFlow.Shared.Contracts.Events.Progress;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Certificates.Application.Features.Certificates.Consumers;

public class CourseCompletedConsumer : IConsumer<CourseCompletedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICertificateRepository _certificateRepository;
    private readonly ILogger<CourseCompletedConsumer> _logger;

    public CourseCompletedConsumer(ILogger<CourseCompletedConsumer> logger, 
        IPublishEndpoint publishEndpoint,
        ICertificateRepository certificateRepository)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
        _certificateRepository = certificateRepository;
    }

    public async Task Consume(ConsumeContext<CourseCompletedEvent> context)
    {
        var message = context.Message;

        var existing = await _certificateRepository
            .GetByStudentAndCourseAsync(message.StudentId, message.CourseId, context.CancellationToken);

        if (existing is not null)
        {
            _logger.LogWarning(
                "Certificate already exists for student {StudentId} course {CourseId} — skipping",
                message.StudentId, message.CourseId);
            return;
        }

        var certificate = Certificate.Create(
            message.StudentId,
            message.CourseId,
            message.CourseTitleSnapshot,
            100);

        await _certificateRepository.InsertAsync(certificate, context.CancellationToken);

        await _publishEndpoint.Publish(new CertificateIssuedEvent
        {
            CertificateId = certificate.Id,
            CertificateNumber = certificate.CertificateNumber,
            StudentId = certificate.StudentId,
            CourseId = certificate.CourseId,
            CourseTitleSnapshot = certificate.CourseTitleSnapshot,
            IssuedAt = certificate.IssuedAt
        }, context.CancellationToken);

        _logger.LogInformation(
            "Certificate {CertificateNumber} issued for student {StudentId} course {CourseId}",
            certificate.CertificateNumber, certificate.StudentId, certificate.CourseId);
    }
}