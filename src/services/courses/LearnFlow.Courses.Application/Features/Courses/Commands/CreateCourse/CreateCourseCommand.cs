using LearnFlow.Courses.Domain.Enums;
using MediatR;

namespace LearnFlow.Courses.Application.Features.Courses.Commands.CreateCourse;

public record CreateCourseCommand(
    string Title,
    string Description,
    string InstructorId,
    string Category,
    CourseLevel Level,
    List<string> Tags,
    decimal Price
) : IRequest<string>;