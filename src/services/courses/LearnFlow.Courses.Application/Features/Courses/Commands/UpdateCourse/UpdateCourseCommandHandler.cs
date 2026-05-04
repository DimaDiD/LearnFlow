using LearnFlow.Courses.Application.Common.Interfaces;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task Handle(UpdateCourseCommand command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {command.CourseId} not found.");

        if (course.InstructorId != command.InstructorId)
            throw new UnauthorizedAccessException("You can only modify your own courses.");

        if (course.Status == Domain.Enums.CourseStatus.Archived)
            throw new InvalidOperationException("Cannot update archived course.");

        course.Update(
            command.Title,
            command.Description,
            command.Category,
            command.Level,
            command.Tags,
            command.Price);

        await _courseRepository.UpdateAsync(course, ct);
    }
}