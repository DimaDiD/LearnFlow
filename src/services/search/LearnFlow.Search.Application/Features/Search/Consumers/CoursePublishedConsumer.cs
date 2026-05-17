using LearnFlow.Search.Application.Common.Interfaces;
using LearnFlow.Search.Domain.Entities;
using LearnFlow.Shared.Contracts.Events.Courses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Search.Application.Features.Search.Consumers;

public class CoursePublishedConsumer : IConsumer<CoursePublishedEvent>
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<CoursePublishedConsumer> _logger;

    public CoursePublishedConsumer(
        ISearchRepository searchRepository,
        ILogger<CoursePublishedConsumer> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CoursePublishedEvent> context)
    {
        var message = context.Message;

        var instructorName = await _searchRepository
            .GetInstructorNameAsync(message.InstructorId, context.CancellationToken)
            ?? string.Empty;

        var document = new CourseDocument
        {
            Id = message.CourseId,
            Title = message.Title,
            Description = message.Description,
            Category = message.Category,
            Level = message.Level,
            Tags = message.Tags,
            InstructorId = message.InstructorId,
            InstructorName = instructorName,
            Price = 0,
            EnrollmentCount = 0,
            IsPublished = true,
            PublishedAt = message.PublishedAt
        };

        await _searchRepository.IndexCourseAsync(document, context.CancellationToken);

        _logger.LogInformation(
            "Course indexed in Elasticsearch: {CourseId} {Title}",
            message.CourseId, message.Title);
    }
}