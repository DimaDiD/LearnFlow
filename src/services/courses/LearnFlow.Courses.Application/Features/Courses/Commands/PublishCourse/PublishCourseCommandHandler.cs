using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Shared.Contracts.Events.Courses;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.PublishCourse;

public class PublishCourseCommandHandler : IRequestHandler<PublishCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PublishCourseCommandHandler> _logger;

    public PublishCourseCommandHandler(
        ICourseRepository courseRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<PublishCourseCommandHandler> logger)
    {
        _courseRepository = courseRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Handle(PublishCourseCommand command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {command.CourseId} not found.");

        if (course.InstructorId != command.InstructorId)
            throw new UnauthorizedAccessException("You can only publish your own courses.");

        course.Publish();
        await _courseRepository.UpdateAsync(course, ct);

        _logger.LogInformation(
            "Course {CourseId} published by instructor {InstructorId}",
            course.Id,
            course.InstructorId);

        await _publishEndpoint.Publish(new CoursePublishedEvent
        {
            CourseId = course.Id,
            InstructorId = course.InstructorId,
            Title = course.Title,
            Description = course.Description,
            Category = course.Category,
            Level = course.Level.ToString(),
            Tags = course.Tags,
            PublishedAt = course.PublishedAt!.Value
        }, ct);

        _logger.LogInformation(
            "CoursePublishedEvent sent for course {CourseId}",
            course.Id);
    }
}