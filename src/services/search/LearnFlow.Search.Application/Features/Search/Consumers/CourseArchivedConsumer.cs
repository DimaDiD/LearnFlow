using LearnFlow.Search.Application.Common.Interfaces;
using LearnFlow.Shared.Contracts.Events.Courses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Search.Application.Features.Search.Consumers;

public class CourseArchivedConsumer : IConsumer<CourseArchivedEvent>
{
    private readonly ISearchRepository _searchRepository;
    private readonly ILogger<CourseArchivedConsumer> _logger;

    public CourseArchivedConsumer(
        ISearchRepository searchRepository,
        ILogger<CourseArchivedConsumer> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CourseArchivedEvent> context)
    {
        var message = context.Message;

        await _searchRepository.DeleteCourseAsync(
            message.CourseId,
            context.CancellationToken);

        _logger.LogInformation(
            "Course removed from Elasticsearch: {CourseId}",
            message.CourseId);
    }
}