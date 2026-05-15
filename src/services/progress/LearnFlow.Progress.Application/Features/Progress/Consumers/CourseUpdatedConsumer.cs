using LearnFlow.Progress.Application.Common.Interfaces;
using LearnFlow.Progress.Domain.Entities;
using LearnFlow.Shared.Contracts.Events.Courses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Progress.Application.Features.Progress.Consumers;

public class CourseUpdatedConsumer : IConsumer<CourseUpdatedEvent>
{
    private readonly ICourseStructureRepository _courseStructureRepository;
    private readonly ILogger<CourseUpdatedConsumer> _logger;

    public CourseUpdatedConsumer(
        ICourseStructureRepository courseStructureRepository,
        ILogger<CourseUpdatedConsumer> logger)
    {
        _courseStructureRepository = courseStructureRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CourseUpdatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Course structure update skipped for course {CourseId} — no structure in event",
            message.CourseId);

        // TODO: додати структуру в CourseUpdatedEvent аналогічно до CoursePublishedEvent
    }
}