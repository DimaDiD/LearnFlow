using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Domain.Entities;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.AddLesson;

public class AddLessonCommandHandler : IRequestHandler<AddLessonCommand, string>
{
    private readonly ICourseRepository _courseRepository;

    public AddLessonCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<string> Handle(AddLessonCommand command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {command.CourseId} not found.");

        if (course.InstructorId != command.InstructorId)
            throw new UnauthorizedAccessException("You can only modify your own courses.");

        var module = course.Modules.FirstOrDefault(m => m.Id == command.ModuleId)
            ?? throw new InvalidOperationException($"Module {command.ModuleId} not found.");

        var lesson = Lesson.Create(
            command.Title,
            command.Description,
            command.VideoUrl,
            command.DurationMinutes,
            command.Order);

        module.AddLesson(lesson);

        await _courseRepository.UpdateAsync(course, ct);

        return lesson.Id;
    }
}