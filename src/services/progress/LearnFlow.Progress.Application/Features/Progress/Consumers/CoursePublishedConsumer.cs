using LearnFlow.Progress.Application.Common.Interfaces;
using LearnFlow.Progress.Domain.Entities;
using LearnFlow.Shared.Contracts.Events.Courses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Progress.Application.Features.Progress.Consumers;

public class CoursePublishedConsumer : IConsumer<CoursePublishedEvent>
{
    private readonly ICourseStructureRepository _courseStructureRepository;
    private readonly ILogger<CoursePublishedConsumer> _logger;

    public CoursePublishedConsumer(
        ICourseStructureRepository courseStructureRepository,
        ILogger<CoursePublishedConsumer> logger)
    {
        _courseStructureRepository = courseStructureRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CoursePublishedEvent> context)
    {
        var message = context.Message;

        var modules = message.Modules.Select(m => new ModuleStructure
        {
            ModuleId = m.ModuleId,
            LessonIds = m.LessonIds
        }).ToList();

        var structure = CourseStructure.Create(message.CourseId, modules);
        await _courseStructureRepository.UpsertAsync(structure, context.CancellationToken);

        _logger.LogInformation(
            "Course structure saved for course {CourseId} with {ModuleCount} modules",
            message.CourseId, modules.Count);
    }
}
