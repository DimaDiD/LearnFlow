using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Domain.Entities;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.AddModule;

public class AddModuleCommandHandler : IRequestHandler<AddModuleCommand, string>
{
    private readonly ICourseRepository _courseRepository;

    public AddModuleCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<string> Handle(AddModuleCommand command, CancellationToken ct)
    {
        var course = await _courseRepository.GetByIdAsync(command.CourseId, ct)
            ?? throw new InvalidOperationException($"Course {command.CourseId} not found.");

        if (course.InstructorId != command.InstructorId)
            throw new UnauthorizedAccessException("You can only modify your own courses.");

        var module = Module.Create(command.Title, command.Description, command.Order);
        course.AddModule(module);

        await _courseRepository.UpdateAsync(course, ct);

        return module.Id;
    }
}