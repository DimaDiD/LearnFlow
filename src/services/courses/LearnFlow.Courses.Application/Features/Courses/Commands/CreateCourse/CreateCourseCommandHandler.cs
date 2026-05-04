using LearnFlow.Courses.Application.Common.Interfaces;
using LearnFlow.Courses.Domain.Entities;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, string>
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseCommandHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<string> Handle(CreateCourseCommand command, CancellationToken ct)
    {
        var course = Course.Create(
            command.Title,
            command.Description,
            command.InstructorId,
            command.Category,
            command.Level,
            command.Tags,
            command.Price);

        await _courseRepository.InsertAsync(course, ct);

        return course.Id;
    }
}