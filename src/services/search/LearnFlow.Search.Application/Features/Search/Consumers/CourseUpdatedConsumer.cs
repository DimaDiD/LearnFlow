using LearnFlow.Search.Application.Common.Interfaces;
using LearnFlow.Search.Domain.Entities;
using LearnFlow.Shared.Contracts.Events.Courses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Search.Application.Features.Search.Consumers;

public class CourseUpdatedConsumer : IConsumer<CourseUpdatedEvent>
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<CourseUpdatedConsumer> _logger;

    public CourseUpdatedConsumer(
        ISearchRepository searchRepository,
        ILogger<CourseUpdatedConsumer> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CourseUpdatedEvent> context)
    {
        var message = context.Message;

        var instructorName = await _searchRepository
            .GetInstructorNameAsync(message.InstructorId ?? string.Empty, context.CancellationToken)
            ?? string.Empty;

        var document = new CourseDocument
        {
            Id = message.CourseId,
            Title = message.Title,
            Description = message.Description,
            Category = message.Category,
            Level = message.Level,
            Tags = message.Tags,
            InstructorId = message.InstructorId ?? string.Empty,
            InstructorName = instructorName,
            IsPublished = true
        };

        await _searchRepository.UpdateCourseAsync(document, context.CancellationToken);

        _logger.LogInformation(
            "Course updated in Elasticsearch: {CourseId}",
            message.CourseId);
    }
}