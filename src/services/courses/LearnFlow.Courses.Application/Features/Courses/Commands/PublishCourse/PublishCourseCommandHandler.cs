using LearnFlow.Courses.Application.Common.Interfaces;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.PublishCourse;

public class PublishCourseCommandHandler : IRequestHandler<PublishCourseCommand>
{
    private readonly ICourseRepository _courseRepository;

    public PublishCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(PublishCourseCommand command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {command.CourseId} not found.");

        if (course.InstructorId != command.InstructorId)
            throw new UnauthorizedAccessException("You can only publish your own courses.");

        course.Publish();

        await _courseRepository.UpdateAsync(course, ct);

        // TODO Phase 3 step 5: publish CoursePublishedEvent to RabbitMQ here
    }
}