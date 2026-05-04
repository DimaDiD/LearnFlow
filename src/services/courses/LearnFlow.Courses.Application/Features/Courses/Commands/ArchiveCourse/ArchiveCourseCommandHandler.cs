using LearnFlow.Courses.Application.Common.Interfaces;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.ArchiveCourse;

public class ArchiveCourseCommandHandler : IRequestHandler<ArchiveCourseCommand>
{
    private readonly ICourseRepository _courseRepository;

    public ArchiveCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(ArchiveCourseCommand command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {command.CourseId} not found.");

        if (course.InstructorId != command.InstructorId)
            throw new UnauthorizedAccessException("You can only archive your own courses.");

        course.Archive();

        await _courseRepository.UpdateAsync(course, ct);
    }
}